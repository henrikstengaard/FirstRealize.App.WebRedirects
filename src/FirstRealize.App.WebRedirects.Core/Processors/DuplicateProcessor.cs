using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class DuplicateProcessor : IProcessor
    {
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlDuplicateOfFirstIndex;
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlDuplicateOfLastIndex;
        private readonly IList<IResult> _results;

        public DuplicateProcessor()
        {
            _oldUrlDuplicateOfFirstIndex =
                new Dictionary<string, IProcessedRedirect>();
            _oldUrlDuplicateOfLastIndex =
                new Dictionary<string, IProcessedRedirect>();
            _results = new List<IResult>();
        }

        public IEnumerable<IResult> Results
        {
            get
            {
                return _results;
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            // old url
            var oldUrl = processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri;

            // detect duplicate of first found old url
            if (_oldUrlDuplicateOfFirstIndex.ContainsKey(oldUrl))
            {
                var duplicateOfFirstResult = new Result
                {
                    Type = ResultTypes.DuplicateOfFirst,
                    Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by first found redirect to new url '{2}'",
                        processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                        processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri,
                        _oldUrlDuplicateOfFirstIndex[oldUrl].Redirect.NewUrl.Parsed.AbsoluteUri),
                    Url = processedRedirect.Redirect.OldUrl
                };
                processedRedirect.Results.Add(
                    duplicateOfFirstResult);
                _results.Add(
                    duplicateOfFirstResult);
            }
            else
            {
                _oldUrlDuplicateOfFirstIndex.Add(oldUrl, processedRedirect);
            }

            // detect duplicate of last found old url
            if (_oldUrlDuplicateOfLastIndex.ContainsKey(oldUrl))
            {
                var duplicateOfLastResult = new Result
                {
                    Type = ResultTypes.DuplicateOfLast,
                    Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by last found redirect to new url '{2}'",
                        processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                        _oldUrlDuplicateOfFirstIndex[oldUrl].Redirect.NewUrl.Parsed.AbsoluteUri,
                        processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri),
                    Url = processedRedirect.Redirect.OldUrl
                };
                _oldUrlDuplicateOfLastIndex[oldUrl].Results.Add(
                    duplicateOfLastResult);
                _results.Add(
                    duplicateOfLastResult);
            }

            _oldUrlDuplicateOfLastIndex[oldUrl] = processedRedirect;
        }
    }
}