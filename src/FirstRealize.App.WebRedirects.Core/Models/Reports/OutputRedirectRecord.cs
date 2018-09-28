namespace FirstRealize.App.WebRedirects.Core.Models.Reports
{
    public class OutputRedirectRecord
    {
        public string OldUrl { get; set; }
        public string NewUrl { get; set; }
        public bool OldUrlHasHost { get; set; }
        public bool NewUrlHasHost { get; set; }
        public string OldUrlParsed { get; set; }
        public string NewUrlParsed { get; set; }
        public string OldUrlOriginal { get; set; }
        public string NewUrlOriginal { get; set; }
    }
}