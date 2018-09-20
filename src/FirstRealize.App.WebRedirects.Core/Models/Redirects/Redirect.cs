namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class Redirect : IRedirect
    {
        public string OldUrl { get; set; }
        public string NewUrl { get; set; }
    }
}