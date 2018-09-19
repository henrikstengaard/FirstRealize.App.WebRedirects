using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public interface IConfiguration
    {
        IEnumerable<string> RedirectCsvFiles { get; }
        Uri DefaultOldUrl { get; }
        Uri DefaultNewUrl { get; }
        IEnumerable<string> OldUrlExcludePatterns { get; }
        IEnumerable<string> NewUrlExcludePatterns { get; }
        bool ForceHttp { get; }
        int MaxRedirectCount { get; }
    }
}