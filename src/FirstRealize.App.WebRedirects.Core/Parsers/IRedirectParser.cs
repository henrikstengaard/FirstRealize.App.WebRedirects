using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IRedirectParser
    {
        IParsedRedirect ParseRedirect(
            IRedirect redirect);
    }
}