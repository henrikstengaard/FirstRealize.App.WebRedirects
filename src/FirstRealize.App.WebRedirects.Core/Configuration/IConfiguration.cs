using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public interface IConfiguration
    {
        IEnumerable<string> RedirectCsvFiles { get; }
        Uri DefaultOldUrl { get; }
        Uri DefaultNewUrl { get; }
        IEnumerable<string> Processors { get; }
        IEnumerable<string> OldUrlExcludePatterns { get; }
        IEnumerable<string> NewUrlExcludePatterns { get; }
        IEnumerable<string> ForceHttpHostPatterns { get; }
        int MaxRedirectCount { get; }
        int SampleCount { get; }
    }
}