using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectProcessingResult : IRedirectProcessingResult
    {
        public IEnumerable<Redirect> Redirects { get; set; }
        public IEnumerable<IProcessedRedirect> ProcessedRedirects { get; set; }
        public IEnumerable<IResult> Results { get; set; }

        public RedirectProcessingResult()
        {
            Redirects = new List<Redirect>();
            ProcessedRedirects = new List<IProcessedRedirect>();
            Results = new List<IResult>();
        }
    }
}