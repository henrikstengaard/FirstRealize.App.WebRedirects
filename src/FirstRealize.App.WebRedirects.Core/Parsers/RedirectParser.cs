using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class RedirectParser : IRedirectParser
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;
        private readonly IUrlFormatter _urlFormatter;

        public RedirectParser(
            IConfiguration configuration,
            IUrlParser urlParser,
            IUrlFormatter urlFormatter)
        {
            _configuration = configuration;
            _urlParser = urlParser;
            _urlFormatter = urlFormatter;
        }

        public IParsedRedirect ParseRedirect(
            IRedirect redirect)
        {
            var oldUrlParsed = _urlParser.Parse(
                    redirect.OldUrl,
                    _configuration.DefaultUrl,
                    true);
            var newUrlParsed = _urlParser.Parse(
                    redirect.NewUrl,
                    _configuration.DefaultUrl,
                    false);

            return new ParsedRedirect
            {
                OldUrl = new Url
                {
                    Raw = redirect.OldUrl,
                    Parsed = oldUrlParsed,
                    Formatted = _urlFormatter.Format(
                        oldUrlParsed)
                },
                NewUrl = new Url
                {
                    Raw = redirect.NewUrl,
                    Parsed = newUrlParsed,
                    Formatted = _urlFormatter.Format(
                        newUrlParsed)
                },
				RedirectType = redirect.RedirectType
            };
        }
    }
}