using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class Configuration
    {
        public string DefaultOldUrl { get; set; }
        public string DefaultNewUrl { get; set; }
        public IEnumerable<string> OldUrlExcludePatterns { get; set; }
        public IEnumerable<string> NewUrlExcludePatterns { get; set; }
    }
}