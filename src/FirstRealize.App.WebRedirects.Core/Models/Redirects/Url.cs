using FirstRealize.App.WebRedirects.Core.Models.Urls;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class Url : IUrl
    {
        public string Raw { get; set; }
        public IParsedUrl Parsed { get; set; }
        public string Formatted { get; set; }
        public bool IsValid
        {
            get
            {
                return Parsed != null &&
                    Parsed.IsValid;
            }
        }
    }
}