using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public interface IConfiguration
    {
        string OutputDir { get; }
        Mode Mode { get; }
        IEnumerable<string> RedirectCsvFiles { get; }
        IParsedUrl DefaultUrl { get; }
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
        string Exporter { get; }
        int HttpClientTimeout { get; }
        RedirectType DefaultRedirectType { get; }
    }
}