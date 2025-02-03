// InfrastructureServiceCollectionExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebScraper.Infra.Data.Contexts;

namespace WebScrapper.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {            
            services.AddDbContext<ScrapingContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ScrapingDb")));
            return services;
        }
    }
}
