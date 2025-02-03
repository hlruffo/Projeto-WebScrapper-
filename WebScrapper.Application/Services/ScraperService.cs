using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using WebScrapper.Application.Interfaces;
using WebScrapper.Domain;
using WebScrapper.Domain.Models;

namespace WebScraperMultiThread.Application
{
    public class WebScraperService : IScraperService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebScraperService> _logger;
        private static readonly string[] UserAgents =
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36"
        };

        public WebScraperService(HttpClient httpClient, ILogger<WebScraperService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ScrapeResult> ScrapeAsync(string url)
        {
            try
            {
                _logger.LogInformation($"Iniciando scraping: {url}");
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.UserAgent.ParseAdd(UserAgents[new Random().Next(UserAgents.Length)]);

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string html = await response.Content.ReadAsStringAsync();

                await SaveHtmlToFileAsync(url, html);

                return ExtractData(html, url);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar {url}: {ex.Message}");
                return new ScrapeResult { NextPageUrl = null };
            }
        }

        private async Task SaveHtmlToFileAsync(string url, string html)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "html_pages");
            Directory.CreateDirectory(directory);

            
            string pageNumber = "1"; 
            if (url.Contains("/page/"))
            {
                try
                {
                    var parts = url.Split(new string[] { "/page/" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        var pagePart = parts[1].Split(new char[] { '/', '?', '&' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        pageNumber = pagePart;
                    }
                }
                catch
                {
                    pageNumber = "1";
                }
            }

            var fileName = $"page_{pageNumber}.html";
            var filePath = Path.Combine(directory, fileName);

            _logger.LogInformation($"Salvando HTML da página em: {filePath}");
            await File.WriteAllTextAsync(filePath, html);
        }

        private ScrapeResult ExtractData(string html, string currentUrl)
        {
            var result = new ScrapeResult();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'table-hover')]/tbody/tr");
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var columns = row.SelectNodes("td");
                    if (columns == null || columns.Count < 7)
                        continue;

                    var proxy = new ProxyDataModel
                    {
                        IpAddress = columns[1].InnerText.Trim(),
                        Port = columns[2].InnerText.Trim(),
                        Country = columns[3].InnerText.Trim(),
                        Protocol = columns[6].InnerText.Trim(),
                        Url = currentUrl,
                        Success = true
                    };
                    result.Proxies.Add(proxy);
                }
                _logger.LogInformation($"Extraídos {result.Proxies.Count} proxies da página: {currentUrl}");
            }
            else
            {
                _logger.LogWarning("Nenhuma tabela de proxies encontrada.");
            }

            var pagination = doc.DocumentNode.SelectSingleNode("//ul[contains(@class, 'pagination')]");
            if (pagination != null)
            {
                var activeLi = pagination.SelectSingleNode(".//li[contains(@class, 'active')]");
                if (activeLi != null)
                {
                    var activePageStr = activeLi.SelectSingleNode(".//a")?.InnerText.Trim();
                    if (int.TryParse(activePageStr, out int activePage))
                    {
                        var nextLi = pagination.SelectSingleNode($".//li[a[text()='{activePage + 1}']]");
                        if (nextLi != null)
                        {
                            var nextLink = nextLi.SelectSingleNode(".//a")?.GetAttributeValue("href", "");
                            if (!string.IsNullOrEmpty(nextLink))
                            {
                                if (!nextLink.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                                {
                                    var uri = new Uri(currentUrl);
                                    nextLink = $"{uri.Scheme}://{uri.Host}{nextLink}";
                                }
                                result.NextPageUrl = nextLink;
                                _logger.LogInformation($"Próxima página encontrada: {result.NextPageUrl}");
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("Elemento de paginação não encontrado.");
            }

            return result;
        }
    }
}
