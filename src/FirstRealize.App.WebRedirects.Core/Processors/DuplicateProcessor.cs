using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class DuplicateProcessor : IProcessor
    {
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlDuplicateOfFirstIndex;
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlDuplicateOfLastIndex;

        public DuplicateProcessor()
        {
            _oldUrlDuplicateOfFirstIndex =
                new Dictionary<string, IProcessedRedirect>();
            _oldUrlDuplicateOfLastIndex =
                new Dictionary<string, IProcessedRedirect>();
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            // old url
            var oldUrl = processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri;

            // detect duplicate of first found old url
            if (_oldUrlDuplicateOfFirstIndex.ContainsKey(oldUrl))
            {
                processedRedirect.Results.Add(
                    new Result
                    {
                        Type = ResultTypes.DuplicateOfFirst,
                        Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by first found redirect to new url '{2}'",
                        processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                        processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri,
                        _oldUrlDuplicateOfFirstIndex[oldUrl].Redirect.NewUrl.Parsed.AbsoluteUri)
                    });
            }
            else
            {
                _oldUrlDuplicateOfFirstIndex.Add(oldUrl, processedRedirect);
            }

            // detect duplicate of last found old url
            if (_oldUrlDuplicateOfLastIndex.ContainsKey(oldUrl))
            {
                _oldUrlDuplicateOfLastIndex[oldUrl].Results.Add(
                    new Result
                    {
                        Type = ResultTypes.DuplicateOfLast,
                        Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by last found redirect to new url '{2}'",
                        processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                        _oldUrlDuplicateOfFirstIndex[oldUrl].Redirect.NewUrl.Parsed.AbsoluteUri,
                        processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri)
                    });
            }

            _oldUrlDuplicateOfLastIndex[oldUrl] = processedRedirect;
        }
    }
}