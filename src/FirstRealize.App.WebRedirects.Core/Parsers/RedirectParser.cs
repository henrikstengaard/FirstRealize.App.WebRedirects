using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;

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

        public void ParseRedirect(
            Redirect redirect)
        {
            // parse old url
            redirect.OldUrl = _urlParser.ParseUrl(
                redirect.OldUrl.Raw,
                _configuration.DefaultOldUrl,
                true);

            // parse new url
            redirect.NewUrl = _urlParser.ParseUrl(
                redirect.NewUrl.Raw,
                _configuration.DefaultNewUrl,
                false);
        }
    }
}