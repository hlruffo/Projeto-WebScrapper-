using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraperMultiThread.Application;
using WebScrapper.Application.Interfaces;

namespace WebScrapper.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
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
            return services;
        }
    }
}
