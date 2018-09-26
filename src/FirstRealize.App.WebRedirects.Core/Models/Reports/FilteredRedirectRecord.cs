namespace FirstRealize.App.WebRedirects.Core.Models.Reports
{
    public class FilteredRedirectRecord
    {
        public string OldUrlRaw { get; set; }
        public bool OldUrlHasHost { get; set; }
        public string OldUrlParsed { get; set; }
        public string NewUrlRaw { get; set; }
        public bool NewUrlHasHost { get; set; }
        public string NewUrlParsed { get; set; }
        public string OldUrlResult { get; set; }
        public string NewUrlResult { get; set; }
    }
}