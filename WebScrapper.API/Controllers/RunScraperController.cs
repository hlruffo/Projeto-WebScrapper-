using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebScrapper.Application.Services;
using WebScrapper.Domain.Models;

namespace WebScraper.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScrapingController : ControllerBase
    {
        private readonly IScraperOrchestrator _orchestrator;

        public ScrapingController(IScraperOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        /// <summary>
        /// Aciona o processo de scraping, salva os resultados e persiste as métricas.
        /// </summary>
        [HttpGet("run")]
        public async Task<ActionResult<List<ProxyDataModel>>> RunScraping()
        {
            var results = await _orchestrator.RunScrapingAsync();
            return Ok(results);
        }
    }
}
