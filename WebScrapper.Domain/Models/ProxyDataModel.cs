namespace WebScrapper.Domain.Models
{
    public class ProxyDataModel
    {
        public string? IpAddress { get; set; }
        public string? Port { get; set; }
        public string? Country { get; set; }
        public string? Protocol { get; set; }
        public string? Url { get; set; }
        public string? Title { get; set; }
        public bool Success { get; set; }   
        public string? ErrorMessage { get; set; }

        public override string ToString()
        {
            return $"{IpAddress}:{Port} ({Protocol}) - {Country}";
        }
    }
}
