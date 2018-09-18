using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class CyclicProcessor : IProcessor, IProcessorPreload
    {
        private readonly IConfiguration _configuration;
        private readonly IDictionary<string, Redirect> _oldUrlIndex;

        public CyclicProcessor(IConfiguration configuration)
        {
            _configuration = configuration;
            _oldUrlIndex = new Dictionary<string, Redirect>();
        }

        private string FormatUrl(string url)
        {
            return _configuration.ForceHttp
                ? Regex.Replace(url, "^https?://", "http://", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                : url;
        }

        public void PreloadRedirects(IEnumerable<Redirect> redirects)
        {
            foreach(var redirect in redirects
                .Where(r => r.IsValid && !r.IsIdentical)
                .ToList())
            {
                var oldUrl = FormatUrl(
                    redirect.OldUrl.Parsed.AbsoluteUri);

                if (_oldUrlIndex.ContainsKey(oldUrl))
                {
                    continue;
                }

                _oldUrlIndex.Add(oldUrl, redirect);
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            var hasRedirect = false;
            var isCyclicRedirect = false;
            var redirectCount = 0;
            var urlsVisited = new List<string>
            {
                FormatUrl(processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri)
            };
            var urlsIndex = new HashSet<string>();

            var url = FormatUrl(
                processedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri);

            do
            {
                redirectCount++;

                if (urlsIndex.Contains(url))
                {
                    isCyclicRedirect = true;
                    break;
                }

                urlsVisited.Add(url);
                urlsIndex.Add(url);

                // in memory / web request switch
                hasRedirect = _oldUrlIndex.ContainsKey(url);

                if (hasRedirect)
                {
                    url = FormatUrl(
                        _oldUrlIndex[url].NewUrl.Parsed.AbsoluteUri);
                }
            } while (hasRedirect && redirectCount < 20);

            if (isCyclicRedirect)
            {
                processedRedirect.Results.Add(
                    new Result
                    {
                        Type = ResultTypes.Cyclic,
                        Message =
                    string.Format(
                        "Cyclic redirect for url '{0}' when visiting urls '{1}'",
                        url,
                        string.Join(",", urlsVisited))
                    });
            }
        }
    }
}