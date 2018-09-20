using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Models.Results
{
    public class Result : IResult
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public IUrl Url { get; set; }
    }
}