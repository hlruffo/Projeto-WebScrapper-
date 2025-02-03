using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebScraper.Infra.Data.Contexts;
using WebScrapper.Application.Extensions;          
using WebScrapper.Infrastructure.Extensions;         
using WebScrapper.Domain.Models;
using WebScraperMultiThread.Application;

class Program
{
    static async Task Main()
    {
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        var services = new ServiceCollection();

        services.AddHttpClient();

        services.AddLogging(configure => configure.AddConsole());

        services.AddApplicationServices();

        services.AddInfrastructureServices(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        var scraperManager = serviceProvider.GetRequiredService<ScraperManager>();

        Console.WriteLine("Iniciando Web Scraper...");
        var startTime = DateTime.Now;

        var results = await scraperManager.RunScrapingAsync(new List<string>
        {
            "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc"
        });

        var endTime = DateTime.Now;

        foreach (var result in results)
            Console.WriteLine($"Extraído: {result}");

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(results, jsonOptions);

        string currentDirectory = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(currentDirectory, "proxies.json");
        await File.WriteAllTextAsync(filePath, json);
        Console.WriteLine($"Arquivo JSON salvo em: {filePath}");

        var metrics = new ScrapingMetrics
        {
            StartDate = startTime,
            EndDate = endTime,
            PagesProcessed = scraperManager.TotalPagesProcessed,
            TotalRowsExtracted = results.Count,
            JsonFileContent = json
        };

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ScrapingContext>();
            context.ScrapingMetrics.Add(metrics);
            await context.SaveChangesAsync();
        }

        Console.WriteLine("Métricas de execução salvas no banco de dados.");
        Console.WriteLine("Scraping finalizado.");
    }
}
