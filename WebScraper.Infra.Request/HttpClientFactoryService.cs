using System;
using System.Net.Http;

namespace WebScraper.Infra.Request
{
    public class HttpClientFactoryService
    {
        //remover cliente estatico
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientFactoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient();
        }
    }
}
