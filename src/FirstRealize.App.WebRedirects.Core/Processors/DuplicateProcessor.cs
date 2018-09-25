using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class DuplicateProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlHelper _urlHelper;
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlDuplicateOfFirstIndex;
        private readonly IDictionary<string, IProcessedRedirect> _oldUrlDuplicateOfLastIndex;
        private readonly IList<IResult> _results;

        public DuplicateProcessor(
            IConfiguration configuration,
            IUrlHelper urlHelper)
        {
            _configuration = configuration;
            _urlHelper = urlHelper;
            _oldUrlDuplicateOfFirstIndex =
                new Dictionary<string, IProcessedRedirect>();
            _oldUrlDuplicateOfLastIndex =
                new Dictionary<string, IProcessedRedirect>();
            _results = new List<IResult>();
        }

        public string Name
        {
            get
            {
                return GetType().Name;
            }
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
            if (!processedRedirect.ParsedRedirect.IsValid)
            {
                return;
            }

            // old url formatted
            var oldUrlFormatted = _urlHelper.FormatUrl(
                processedRedirect.ParsedRedirect.OldUrl.Parsed);

            switch (_configuration.DuplicateOldUrlStrategy)
            {
                case DuplicateUrlStrategy.KeepFirst:
                    DetectDuplicateOfFirst(oldUrlFormatted, processedRedirect);
                    break;
                case DuplicateUrlStrategy.KeepLast:
                    DetectDuplicateOfLast(oldUrlFormatted, processedRedirect);
                    break;
            }
        }

        private void DetectDuplicateOfFirst(
            string oldUrlFormatted,
            IProcessedRedirect processedRedirect)
        {
            if (_oldUrlDuplicateOfFirstIndex.ContainsKey(oldUrlFormatted))
            {
                var duplicateOfFirstResult = new Result
                {
                    Type = ResultTypes.DuplicateOfFirst,
                    Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by first found redirect to new url '{2}'",
                        processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri,
                        processedRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri,
                        _oldUrlDuplicateOfFirstIndex[oldUrlFormatted].ParsedRedirect.NewUrl.Parsed.AbsoluteUri),
                    Url = processedRedirect.ParsedRedirect.OldUrl
                };
                processedRedirect.Results.Add(
                    duplicateOfFirstResult);
                _results.Add(
                    duplicateOfFirstResult);
            }
            else
            {
                _oldUrlDuplicateOfFirstIndex.Add(
                    oldUrlFormatted,
                    processedRedirect);
            }
        }
        private void DetectDuplicateOfLast(
            string oldUrlFormatted,
            IProcessedRedirect processedRedirect)
        {
            if (_oldUrlDuplicateOfLastIndex.ContainsKey(oldUrlFormatted))
            {
                var duplicateOfLastResult = new Result
                {
                    Type = ResultTypes.DuplicateOfLast,
                    Message =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Redirect to new url '{1}' skipped by last found redirect to new url '{2}'",
                        processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri,
                        _oldUrlDuplicateOfLastIndex[oldUrlFormatted].ParsedRedirect.NewUrl.Parsed.AbsoluteUri,
                        processedRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri),
                    Url = processedRedirect.ParsedRedirect.OldUrl
                };
                _oldUrlDuplicateOfLastIndex[oldUrlFormatted].Results.Add(
                    duplicateOfLastResult);
                _results.Add(
                    duplicateOfLastResult);
            }

            _oldUrlDuplicateOfLastIndex[oldUrlFormatted] = processedRedirect;
        }
    }
}