using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebScrapper.Application.Interfaces;
using WebScrapper.Application.Services;
using WebScraperMultiThread.Application;

namespace WebScrapper.Application.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IScraperService, WebScraperService>();
            services.AddSingleton<ScraperManager>(provider =>
                new ScraperManager(
                    provider.GetRequiredService<IScraperService>(),
                    provider.GetRequiredService<ILogger<ScraperManager>>(),
                    maxConcurrentTasks: 3
                ));

            services.AddScoped<IScraperOrchestrator, ScrapingOrchestrator>();

            return services;
        }
    }
}
