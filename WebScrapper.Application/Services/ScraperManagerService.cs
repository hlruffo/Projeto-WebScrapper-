// ScraperManager.cs (trecho modificado)
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebScrapper.Application.Interfaces;
using WebScrapper.Domain;
using WebScrapper.Domain.Models;

namespace WebScraperMultiThread.Application
{
    public class ScraperManager
    {
        private readonly IScraperService _scraperService;
        private readonly SemaphoreSlim _semaphore;
        private readonly ILogger<ScraperManager> _logger;

        // Variável para acumular a quantidade de páginas processadas
        private int _totalPagesProcessed = 0;
        public int TotalPagesProcessed => _totalPagesProcessed;

        public ScraperManager(IScraperService scraperService, ILogger<ScraperManager> logger, int maxConcurrentTasks)
        {
            _scraperService = scraperService;
            _logger = logger;
            _semaphore = new SemaphoreSlim(maxConcurrentTasks);
        }

        public async Task<List<ProxyDataModel>> RunScrapingAsync(List<string> urls)
        {
            var results = new List<ProxyDataModel>();
            var tasks = new List<Task>();

            foreach (var baseUrl in urls)
            {
                // Para cada URL base, processa a paginação de forma sequencial
                tasks.Add(Task.Run(async () =>
                {
                    string currentUrl = baseUrl;
                    while (!string.IsNullOrEmpty(currentUrl))
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            var scrapeResult = await _scraperService.ScrapeAsync(currentUrl);

                            // Atualiza o contador de páginas processadas (de forma thread-safe)
                            Interlocked.Increment(ref _totalPagesProcessed);

                            // Adiciona os proxies extraídos (acesso sincronizado à lista compartilhada)
                            lock (results)
                            {
                                results.AddRange(scrapeResult.Proxies);
                            }

                            // Atualiza a URL para a próxima página (se houver)
                            currentUrl = scrapeResult.NextPageUrl;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Erro ao processar a URL {currentUrl}: {ex.Message}");
                            break;
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results;
        }
    }
}
