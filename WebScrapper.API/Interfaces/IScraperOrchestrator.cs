using System.Collections.Generic;
using System.Threading.Tasks;
using WebScrapper.Domain.Models;

namespace WebScrapper.Application.Services
{
    public interface IScraperOrchestrator
    {
        Task<List<ProxyDataModel>> RunScrapingAsync();
    }
}
