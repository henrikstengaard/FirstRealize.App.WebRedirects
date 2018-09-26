namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class Redirect : IRedirect
    {
        public string OldUrl { get; set; }
        public string NewUrl { get; set; }
        public bool OldUrlHasHost { get; set; }
        public bool NewUrlHasHost { get; set; }
        public string OldUrlParsed { get; set; }
        public string NewUrlParsed { get; set; }
        public string OldUrlRefined { get; set; }
        public string NewUrlRefined { get; set; }
    }
}