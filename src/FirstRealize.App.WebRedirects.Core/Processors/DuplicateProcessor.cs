using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class DuplicateProcessor : IProcessor
    {
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlIndex;

        public DuplicateProcessor()
        {
            _oldUrlIndex = new Dictionary<string, IProcessedRedirect>();
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            var oldUrl = processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri;
            if (_oldUrlIndex.ContainsKey(oldUrl))
            {
                processedRedirect.Results.Add(
                    new Result
                    {
                        Type = ResultTypes.Duplicate,
                        Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by first found redirect to new url '{2}'",
                        processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                        processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri,
                        _oldUrlIndex[oldUrl].Redirect.NewUrl.Parsed.AbsoluteUri)
                    });
            }
            else
            {
                _oldUrlIndex[oldUrl] = processedRedirect;
            }
        }
    }
}