namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class OutputRedirect
    {
        public string OldUrl { get; set; }
        public string NewUrl { get; set; }
        public bool ValidMatchingOriginalNewUrl { get; set; }
        public bool ValidNotMatchingOriginalNewUrl { get; set; }
    }
}