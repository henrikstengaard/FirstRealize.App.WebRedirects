using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Models.Results
{
    public interface IResult
    {
        string Type { get; }
        string Message { get; }
        IUrl Url { get; }
    }
}