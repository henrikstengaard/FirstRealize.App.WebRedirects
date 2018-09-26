using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;

namespace FirstRealize.App.WebRedirects.Test.EngineTests
{
    class TestProcessor : IProcessor
    {
        public string Name => GetType().Name;

        public IConfiguration Configuration { get; set; }

        public IEnumerable<IResult> Results
        {
            get
            {
                return new List<IResult>();
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            processedRedirect.Results.Add(
                new Result
                {
                    Type = Name,
                    Message = Name,
                    Url = processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri
                });
        }
    }
}