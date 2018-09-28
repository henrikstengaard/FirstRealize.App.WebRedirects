using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public interface IConfiguration
    {
        IEnumerable<string> RedirectCsvFiles { get; }
        Uri DefaultUrl { get; }
        IEnumerable<string> Processors { get; }
        IEnumerable<string> OldUrlExcludePatterns { get; }
        IEnumerable<string> NewUrlExcludePatterns { get; }
        DuplicateUrlStrategy DuplicateOldUrlStrategy { get; }
        bool ExcludeOldUrlRootRedirects { get; }
        bool UseTestHttpClient { get; }
        int? TestHttpClientNewUrlStatusCode { get; }
        IEnumerable<string> ForceHttpHostPatterns { get; }
        int MaxRedirectCount { get; }
        int SampleCount { get; }
        bool Export { get; }
    }
}