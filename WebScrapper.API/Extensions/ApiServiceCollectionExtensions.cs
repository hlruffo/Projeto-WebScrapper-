using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebScrapper.Application.Interfaces;
using WebScrapper.Application.Services;  // Novo namespace para o orquestrador
using WebScraperMultiThread.Application;

namespace WebScrapper.Application.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registra o serviço de scraping e o gerenciador de scraping.
            services.AddSingleton<IScraperService, WebScraperService>();
            services.AddSingleton<ScraperManager>(provider =>
                new ScraperManager(
                    provider.GetRequiredService<IScraperService>(),
                    provider.GetRequiredService<ILogger<ScraperManager>>(),
                    maxConcurrentTasks: 3
                ));

            // Registra o orquestrador que encapsula toda a lógica de scraping, persistência de arquivos e métricas.
            services.AddScoped<IScraperOrchestrator, ScrapingOrchestrator>();

            return services;
        }
    }
}
