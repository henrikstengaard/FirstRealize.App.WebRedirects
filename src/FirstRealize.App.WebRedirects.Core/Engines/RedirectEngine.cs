using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Readers;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectEngine
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;

        public RedirectEngine(
            IConfiguration configuration,
            IUrlParser urlParser)
        {
            _configuration = configuration;
            _urlParser = urlParser;
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