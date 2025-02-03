using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebScrapper.Domain.Models;
using WebScraperMultiThread.Application;
using WebScraper.Infra.Data.Contexts;

namespace WebScrapper.Application.Services
{
    public class ScrapingOrchestrator : IScraperOrchestrator
    {
        private readonly ScraperManager _scraperManager;
        private readonly ScrapingContext _context;
        private readonly ILogger<ScrapingOrchestrator> _logger;

        public ScrapingOrchestrator(
            ScraperManager scraperManager,
            ScrapingContext context,
            ILogger<ScrapingOrchestrator> logger)
        {
            _scraperManager = scraperManager;
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProxyDataModel>> RunScrapingAsync()
        {
            // Defina a(s) URL(s) para o scraping (pode ser parametrizado se necessário)
            var urls = new List<string>
            {
                "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc"
            };

            var startTime = DateTime.Now;

            // Chama o ScraperManager para executar o scraping
            var results = await _scraperManager.RunScrapingAsync(urls);

            var endTime = DateTime.Now;

            // Serializa os resultados para JSON (com indentação)
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(results, jsonOptions);

            // Salva o arquivo JSON na pasta atual
            string currentDirectory = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentDirectory, "proxies.json");
            await File.WriteAllTextAsync(filePath, json);
            _logger.LogInformation($"Arquivo JSON salvo em: {filePath}");

            // Cria o objeto com as métricas de execução
            var metrics = new ScrapingMetrics
            {
                StartDate = startTime,
                EndDate = endTime,
                PagesProcessed = _scraperManager.TotalPagesProcessed,
                TotalRowsExtracted = results.Count,
                JsonFileContent = json
            };

            // Persiste as métricas no banco de dados
            _context.ScrapingMetrics.Add(metrics);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Métricas de execução salvas no banco de dados.");

            return results;
        }
    }
}
