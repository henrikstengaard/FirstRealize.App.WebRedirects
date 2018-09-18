using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class RedirectParser
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;

        public RedirectParser(
            IConfiguration configuration,
            IUrlParser urlParser)
        {
            _configuration = configuration;
            _urlParser = urlParser;
        }

        public Redirect ParseRedirect(
            string oldUrl,
            string newUrl)
        {
            return new Redirect
            {
                OldUrl = new Url
                {
                    Raw = oldUrl,
                    Parsed = _urlParser.ParseUrl(
                        oldUrl,
                        _configuration.DefaultOldUrl,
                        true)
                },
                NewUrl = new Url
                {
                    Raw = newUrl,
                    Parsed = _urlParser.ParseUrl(
                        newUrl,
                        _configuration.DefaultNewUrl,
                        false)
                }
            };
        }
    }
}