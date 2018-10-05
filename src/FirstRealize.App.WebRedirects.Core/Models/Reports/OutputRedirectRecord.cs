namespace FirstRealize.App.WebRedirects.Core.Models.Reports
{
    public class OutputRedirectRecord
    {
        public string OldUrl { get; set; }
        public bool OldUrlHasHost { get; set; }
        public string NewUrl { get; set; }
        public bool NewUrlHasHost { get; set; }
        public string ParsedOldUrl { get; set; }
        public string ParsedNewUrl { get; set; }
        public string OriginalOldUrl { get; set; }
        public bool OriginalOldUrlHasHost { get; set; }
        public string OriginalNewUrl { get; set; }
        public bool OriginalNewUrlHasHost { get; set; }
    }
}