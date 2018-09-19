using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class ExcludeProcessor : IProcessor
    {
        private readonly IConfiguration _configuration;

        private readonly IList<Result> _results;

        public ExcludeProcessor(
            IConfiguration configuration)
        {
            _configuration = configuration;
            _results = new List<Result>();
        }

        public IEnumerable<Result> Results
        {
            get
            {
                return _results;
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            var oldUrlPatternMatches = GetMatchingUrlPatterns(
                processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                _configuration.OldUrlExcludePatterns)
                .ToList();

            var newUrlPatternMatches = GetMatchingUrlPatterns(
                processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri,
                _configuration.NewUrlExcludePatterns)
                .ToList();

            if (!oldUrlPatternMatches.Any() &&
                !newUrlPatternMatches.Any())
            {
                return;
            }

            var message = new StringBuilder(
                "Excluded redirect matches ");

            if (oldUrlPatternMatches.Any())
            {
                message.Append(
                    string.Format(
                        "old url exclude patterns '{0}'", 
                        string.Join(",", oldUrlPatternMatches)));
            }

            if (newUrlPatternMatches.Any())
            {
                if (oldUrlPatternMatches.Any())
                {
                    message.Append(" and ");
                }

                message.Append(
                    string.Format(
                        "new url exclude patterns '{0}'",
                        string.Join(",", newUrlPatternMatches)));
            }

            processedRedirect.Results.Add(
                new Result
                {
                    Type = ResultTypes.Excluded,
                    Message = message.ToString()
                });
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