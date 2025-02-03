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
            var urls = new List<string>
            {
                "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc"
            };

            var startTime = DateTime.Now;
           
            var results = await _scraperManager.RunScrapingAsync(urls);

            var endTime = DateTime.Now;

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(results, jsonOptions);

            string currentDirectory = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentDirectory, "proxies.json");
            await File.WriteAllTextAsync(filePath, json);
            _logger.LogInformation($"Arquivo JSON salvo em: {filePath}");

            var metrics = new ScrapingMetrics
            {
                StartDate = startTime,
                EndDate = endTime,
                PagesProcessed = _scraperManager.TotalPagesProcessed,
                TotalRowsExtracted = results.Count,
                JsonFileContent = json
            };

            _context.ScrapingMetrics.Add(metrics);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Métricas de execução salvas no banco de dados.");

            return results;
        }
    }
}
