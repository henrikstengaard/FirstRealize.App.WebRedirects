using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class IdenticalProcessor : IProcessor
    {
        private readonly IList<IResult> _results;

        public IdenticalProcessor()
        {
            _results = new List<IResult>();
        }

        public string Name => GetType().Name;

        public IEnumerable<IResult> Results
        {
            get
            {
                return _results;
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            if (!processedRedirect.ParsedRedirect.IsIdentical)
            {
                return;
            }

            var identicalResult = new Result
            {
                Type = ResultTypes.IdenticalResult,
                Message =
                string.Format(
                    "Identical redirect of from and to url '{0}'",
                    processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri),
                Url = processedRedirect.ParsedRedirect.OldUrl
            };
            processedRedirect.Results.Add(
                identicalResult);
            _results.Add(
                identicalResult);
        }
    }
}