using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class RedirectParser : IRedirectParser
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

        public IParsedRedirect ParseRedirect(
            IRedirect redirect)
        {
            return new ParsedRedirect
            {
                OldUrl = _urlParser.ParseUrl(
                    redirect.OldUrl,
                    _configuration.DefaultOldUrl,
                    true),
                NewUrl = _urlParser.ParseUrl(
                    redirect.NewUrl,
                    _configuration.DefaultNewUrl,
                    false)
            };
        }
    }
}