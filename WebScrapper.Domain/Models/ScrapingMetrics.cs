﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper.Domain.Models
{
    public class ScrapingMetrics
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PagesProcessed { get; set; }
        public int TotalRowsExtracted { get; set; }
        public string? JsonFileContent { get; set; }
    }
}
