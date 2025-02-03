using System.Threading.Tasks;
using WebScrapper.Domain;
using WebScrapper.Domain.Models;

namespace WebScrapper.Application.Interfaces
{
    public interface IScraperService
    {
        Task<ScrapeResult> ScrapeAsync(string url);
    }
}
