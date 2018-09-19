using FirstRealize.App.WebRedirects.Core.Models;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IRedirectParser
    {
        void ParseRedirect(
            Redirect redirect);
    }
}