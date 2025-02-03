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
            // Registra o DbContext utilizando a connection string definida no appsettings.json.
            services.AddDbContext<ScrapingContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ScrapingDb")));
            return services;
        }
    }
}
