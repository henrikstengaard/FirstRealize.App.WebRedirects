using System.Collections.Generic;
using System.Text;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class InvalidProcessor : IProcessor
    {
        private readonly IList<IResult> _results;

        public InvalidProcessor()
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
            if (processedRedirect.ParsedRedirect.IsValid)
            {
                return;
            }

            var oldUrlInvalid = false;
            var messageBuilder = new StringBuilder("Invalid ");
            if (processedRedirect.ParsedRedirect.OldUrl != null && !processedRedirect.ParsedRedirect.OldUrl.IsValid)
            {
                oldUrlInvalid = true;
                messageBuilder.Append(string.Format(
                    "old url '{0}'",
                    processedRedirect.ParsedRedirect.OldUrl.Raw));
            }
            if (processedRedirect.ParsedRedirect.NewUrl != null && !processedRedirect.ParsedRedirect.NewUrl.IsValid)
            {
                if (oldUrlInvalid)
                {
                    messageBuilder.Append(" and ");
                }
                messageBuilder.Append(string.Format(
                    "new url '{0}'",
                    processedRedirect.ParsedRedirect.NewUrl.Raw));
            }

            var invalidResult = new Result
            {
                Type = ResultTypes.InvalidResult,
                Message = messageBuilder.ToString(),
                Url = processedRedirect.ParsedRedirect.OldUrl
            };
            processedRedirect.Results.Add(
                invalidResult);
            _results.Add(
                invalidResult);
        }
    }
}