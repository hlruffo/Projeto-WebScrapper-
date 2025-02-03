using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Infra.Data.Contexts;

namespace WebScraper.Infra.Data.Factory
{
    public class ScrapingContextFactory : IDesignTimeDbContextFactory<ScrapingContext>
    {
        public ScrapingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScrapingContext>();
            // Configure a connection string adequada para o ambiente de design time.
            // Se estiver rodando fora do Docker, use "localhost"; se estiver dentro, use o nome do serviço.
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ScrapingDb;User Id=sa;Password=ELawScrapper;");

            return new ScrapingContext(optionsBuilder.Options);
        }
    }
}
