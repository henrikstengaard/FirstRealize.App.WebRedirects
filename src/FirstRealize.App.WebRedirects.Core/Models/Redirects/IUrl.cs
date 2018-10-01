using FirstRealize.App.WebRedirects.Core.Models.Urls;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IUrl
    {
        string Raw { get; }
        IParsedUrl Parsed { get; }
        string Formatted { get; }
        bool IsValid { get; }
    }
}