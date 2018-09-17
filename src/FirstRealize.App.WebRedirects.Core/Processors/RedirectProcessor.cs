using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class RedirectProcessor
    {
        private readonly Configuration _configuration;
        private readonly IList<IProcessor> _processors;

        public RedirectProcessor(
            Configuration configuration)
        {
            _configuration = configuration;
            _processors = new List<IProcessor>
            {
                new DuplicateProcessor()
            };
        }

        public IProcessedRedirect Process(
            Redirect redirect)
        {
            var processedRedirect = new ProcessedRedirect
            {
                Redirect = redirect
            };

            if (!redirect.IsValid ||
                redirect.IsIdentical)
            {
                return processedRedirect;
            }

            foreach(var processor in _processors)
            {
                processor.Process(processedRedirect);
            }

            return processedRedirect;
        }

        public bool ExcludeUrls(ProcessedRedirect processedRedirect)
        {
            return IsUrlExcluded(processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri,
                _configuration.OldUrlExcludePatterns);
        }

        private bool IsUrlExcluded(
            string url,
            IEnumerable<string> urlExcludePatterns)
        {
            return urlExcludePatterns.Any(x => Regex.IsMatch(
                url, x, RegexOptions.IgnoreCase | RegexOptions.Compiled));
        }
    }
}