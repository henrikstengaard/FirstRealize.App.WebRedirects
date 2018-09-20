namespace FirstRealize.App.WebRedirects.Core.Models.Results
{
    public class UrlResponseResult : Result
    {
        public int StatusCode { get; set; }
        public string Location { get; set; }
    }
}