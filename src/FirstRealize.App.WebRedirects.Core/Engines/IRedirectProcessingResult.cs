using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public interface IRedirectProcessingResult
    {
        IEnumerable<IProcessor> Processors { get; }
        IEnumerable<IRedirect> Redirects { get; }
        IEnumerable<IParsedRedirect> ParsedRedirects { get; }
        IEnumerable<IProcessedRedirect> ProcessedRedirects { get; }
        IEnumerable<IResult> Results { get; }
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
    }
}