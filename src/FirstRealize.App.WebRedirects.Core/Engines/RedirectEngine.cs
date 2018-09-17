using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Readers;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectEngine
    {
        private readonly Configuration _configuration;
        private readonly UrlParser _urlParser;

        public RedirectEngine(Configuration configuration)
        {
            _configuration = configuration;
            _urlParser = new UrlParser();
        }

        public void Run()
        {
            var redirects = LoadRedirects();

            ParseUrls(redirects);
        }

        private IEnumerable<Redirect> LoadRedirects()
        {
            var redirects = new List<Redirect>();

            foreach (var redirectsCsvFile in _configuration.RedirectCsvFiles)
            {
                using (var redirectCsvReader = 
                    new RedirectCsvReader(redirectsCsvFile))
                {
                    redirects.AddRange(
                        redirectCsvReader.ReadAllRedirects());
                }
            }

            return redirects;
        }

        private void ParseUrls(IEnumerable<Redirect> redirects)
        {
            foreach(var redirect in redirects.ToList())
            {
                redirect.OldUrl.Parsed = _urlParser.ParseUrl(
                    redirect.OldUrl.Raw);
                redirect.NewUrl.Parsed = _urlParser.ParseUrl(
                    redirect.NewUrl.Raw);
            }
        }
    }
}