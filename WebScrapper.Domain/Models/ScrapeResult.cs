using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper.Domain.Models
{
    public class ScrapeResult
    {
        public List<ProxyDataModel> Proxies { get; set; } = new List<ProxyDataModel>();
        public string? NextPageUrl { get; set; }
    }
}
