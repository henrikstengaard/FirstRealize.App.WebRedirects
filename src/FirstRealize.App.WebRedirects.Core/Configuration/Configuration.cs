﻿using Newtonsoft.Json;
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
        public IEnumerable<string> OldUrlExcludePatterns { get; set; }
        public IEnumerable<string> NewUrlExcludePatterns { get; set; }
        public bool ForceHttp { get; set; }
    }
}