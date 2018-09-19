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
            redirect.OldUrl.Parsed = _urlParser.ParseUrl(
                redirect.OldUrl.Raw,
                _configuration.DefaultOldUrl,
                true);

            redirect.NewUrl.Parsed = _urlParser.ParseUrl(
                redirect.NewUrl.Raw,
                _configuration.DefaultNewUrl,
                false);
        }
    }
}