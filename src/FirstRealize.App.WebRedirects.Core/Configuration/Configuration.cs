using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public class Configuration : IConfiguration
    {
        public IEnumerable<string> RedirectCsvFiles { get; set; }
        [JsonConverter(typeof(UriJsonConverter))]
        public Uri DefaultOldUrl { get; set; }
        [JsonConverter(typeof(UriJsonConverter))]
        public Uri DefaultNewUrl { get; set; }
        public IEnumerable<string> Processors { get; set; }
        public IEnumerable<string> OldUrlExcludePatterns { get; set; }
        public IEnumerable<string> NewUrlExcludePatterns { get; set; }
        public bool UseTestHttpClient { get; set; }
        public IEnumerable<string> ForceHttpHostPatterns { get; set; }
        public int MaxRedirectCount { get; set; }
        public int SampleCount { get; set; }

        public Configuration()
        {
            RedirectCsvFiles = new List<string>();
            Processors = new List<string>();
            OldUrlExcludePatterns = new List<string>();
            NewUrlExcludePatterns = new List<string>();
            ForceHttpHostPatterns = new List<string>();
            MaxRedirectCount = 20;
            SampleCount = 0;
        }
    }
}