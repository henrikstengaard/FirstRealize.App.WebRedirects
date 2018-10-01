using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class ExcludeProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IList<IResult> _results;

        public ExcludeProcessor(
            IConfiguration configuration)
        {
            _configuration = configuration;
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

            var oldUrlPatternMatches = GetMatchingUrlPatterns(
                processedRedirect.ParsedRedirect.OldUrl.Formatted,
                _configuration.OldUrlExcludePatterns)
                .ToList();

            var newUrlPatternMatches = GetMatchingUrlPatterns(
                processedRedirect.ParsedRedirect.NewUrl.Formatted,
                _configuration.NewUrlExcludePatterns)
                .ToList();

            if (!oldUrlPatternMatches.Any() &&
                !newUrlPatternMatches.Any())
            {
                return;
            }

            var message = new StringBuilder(
                "Excluded redirect matches ");

            IUrl url = null;

            if (oldUrlPatternMatches.Any())
            {
                message.Append(
                    string.Format(
                        "old url exclude patterns '{0}'", 
                        string.Join(",", oldUrlPatternMatches)));
                url = processedRedirect.ParsedRedirect.OldUrl;
            }

            if (newUrlPatternMatches.Any())
            {
                if (oldUrlPatternMatches.Any())
                {
                    message.Append(" and ");
                }
                {
                    url = processedRedirect.ParsedRedirect.NewUrl;
                }

                message.Append(
                    string.Format(
                        "new url exclude patterns '{0}'",
                        string.Join(",", newUrlPatternMatches)));
            }

            var excludedRedirectResult = new Result
            {
                Type = ResultTypes.ExcludedRedirect,
                Message = message.ToString(),
                Url = url.Formatted
            };
            processedRedirect.Results.Add(
                excludedRedirectResult);
            _results.Add(excludedRedirectResult);
        }

        private IEnumerable<string> GetMatchingUrlPatterns(
            string url,
            IEnumerable<string> urlPatterns)
        {
            return urlPatterns.Where(x => Regex.IsMatch(
                url, x, RegexOptions.IgnoreCase | RegexOptions.Compiled));
        }
    }
}