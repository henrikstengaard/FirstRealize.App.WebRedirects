using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class Configuration
    {
        public IEnumerable<string> RedirectCsvFiles { get; set; }
        public string DefaultOldUrl { get; set; }
        public string DefaultNewUrl { get; set; }
        public IEnumerable<string> OldUrlExcludePatterns { get; set; }
        public IEnumerable<string> NewUrlExcludePatterns { get; set; }
        public bool ForceHttp { get; set; }
    }
}