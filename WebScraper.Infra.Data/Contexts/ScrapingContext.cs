using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScrapper.Domain.Models;

namespace WebScraper.Infra.Data.Contexts
{
    public class ScrapingContext:DbContext
    {
        public ScrapingContext(DbContextOptions<ScrapingContext> options) : base(options) { }
        public DbSet<ScrapingMetrics> ScrapingMetrics { get; set; }
    }
}
