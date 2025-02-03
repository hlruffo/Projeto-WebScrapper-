using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebScraper.Infra.Data.Contexts;
using WebScrapper.Application.Extensions;          // Para AddApplicationServices()
using WebScrapper.Infrastructure.Extensions;         // Para AddInfrastructureServices()
using WebScrapper.Domain.Models;
using WebScraperMultiThread.Application;

class Program
{
    static async Task Main()
    {

        // Carrega a configuração (appsettings.json)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Cria o container de serviços
        var services = new ServiceCollection();

        // Registra o HttpClient
        services.AddHttpClient();

        // Registra o Logging
        services.AddLogging(configure => configure.AddConsole());

        // Registra os serviços da camada Application
        services.AddApplicationServices();

        // Registra os serviços da camada Infrastructure, passando a configuração
        services.AddInfrastructureServices(configuration);

        // Constrói o ServiceProvider
        using var serviceProvider = services.BuildServiceProvider();

        // Obtém o ScraperManager a partir do container
        var scraperManager = serviceProvider.GetRequiredService<ScraperManager>();

        Console.WriteLine("Iniciando Web Scraper...");
        var startTime = DateTime.Now;

        // Inicia o scraping para a(s) URL(s) desejada(s)
        var results = await scraperManager.RunScrapingAsync(new List<string>
        {
            "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc"
        });

        var endTime = DateTime.Now;

        // Exibe os resultados no console
        foreach (var result in results)
            Console.WriteLine($"Extraído: {result}");

        // Serializa os resultados para JSON (com indentação)
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(results, jsonOptions);

        // Salva o arquivo JSON na pasta atual
        string currentDirectory = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(currentDirectory, "proxies.json");
        await File.WriteAllTextAsync(filePath, json);
        Console.WriteLine($"Arquivo JSON salvo em: {filePath}");

        // Cria o objeto com as métricas de execução
        var metrics = new ScrapingMetrics
        {
            StartDate = startTime,
            EndDate = endTime,
            PagesProcessed = scraperManager.TotalPagesProcessed,
            TotalRowsExtracted = results.Count,
            JsonFileContent = json
        };

        // Salva as métricas no banco de dados
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
