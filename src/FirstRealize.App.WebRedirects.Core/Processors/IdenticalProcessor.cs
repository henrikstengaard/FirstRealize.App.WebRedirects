using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class IdenticalProcessor : IProcessor
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IList<IResult> _results;

        public IdenticalProcessor(
            IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
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
            if (!processedRedirect.ParsedRedirect.IsValid)
            {
                return;
            }

            if (!_urlHelper.AreIdentical(
                processedRedirect.ParsedRedirect.OldUrl.Formatted,
                processedRedirect.ParsedRedirect.NewUrl.Formatted))
            {
                return;
            }

            var identicalResult = new Result
            {
                Type = ResultTypes.IdenticalResult,
                Message =
                string.Format(
                    "Identical redirect of from and to url '{0}'",
                    processedRedirect.ParsedRedirect.OldUrl.Formatted),
                Url = processedRedirect.ParsedRedirect.OldUrl.Formatted
            };
            processedRedirect.Results.Add(
                identicalResult);
            _results.Add(
                identicalResult);
        }
    }
}