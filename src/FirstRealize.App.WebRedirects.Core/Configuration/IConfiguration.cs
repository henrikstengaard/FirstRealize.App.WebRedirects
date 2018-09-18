using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public interface IConfiguration
    {
        IEnumerable<string> RedirectCsvFiles { get; set; }
        Uri DefaultOldUrl { get; set; }
        Uri DefaultNewUrl { get; set; }
        IEnumerable<string> OldUrlExcludePatterns { get; set; }
        IEnumerable<string> NewUrlExcludePatterns { get; set; }
        bool ForceHttp { get; set; }
    }
}