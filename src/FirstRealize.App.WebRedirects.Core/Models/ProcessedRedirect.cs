using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class ProcessedRedirect : IProcessedRedirect
    {
        public IParsedRedirect ParsedRedirect { get; set; }
        public IList<IResult> Results { get; set; }

        public ProcessedRedirect()
        {
            Results = new List<IResult>();
        }
    }
}