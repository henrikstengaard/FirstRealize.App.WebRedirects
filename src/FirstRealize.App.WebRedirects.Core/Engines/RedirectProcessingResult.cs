using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectProcessingResult : IRedirectProcessingResult
    {
        public IEnumerable<IProcessor> Processors { get; set; }

        public IEnumerable<IRedirect> Redirects { get; set; }
        public IEnumerable<IParsedRedirect> ParsedRedirects { get; set; }
        public IEnumerable<IProcessedRedirect> ProcessedRedirects { get; set; }
        public IEnumerable<IResult> Results { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public RedirectProcessingResult()
        {
            Processors = new List<IProcessor>();
            Redirects = new List<IRedirect>();
            ParsedRedirects = new List<IParsedRedirect>();
            ProcessedRedirects = new List<IProcessedRedirect>();
            Results = new List<IResult>();
        }
    }
}