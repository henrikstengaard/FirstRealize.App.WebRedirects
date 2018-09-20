using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public interface IRedirectProcessingResult
    {
        IEnumerable<IRedirect> Redirects { get; }
        IEnumerable<IParsedRedirect> ParsedRedirects { get; }
        IEnumerable<IProcessedRedirect> ProcessedRedirects { get; }
        IEnumerable<IResult> Results { get; }
    }
}