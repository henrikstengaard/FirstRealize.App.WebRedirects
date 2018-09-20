namespace FirstRealize.App.WebRedirects.Core.Models.Reports
{
    public class ProcessedRedirectRecord
    {
        public string OldUrlRaw { get; set; }
        public string NewUrlRaw { get; set; }
        public string OldUrlParsed { get; set; }
        public string NewUrlParsed { get; set; }

        public bool Valid { get; set; }
        public bool Identical { get; set; }

        public int ResultCount { get; set; }
        public string ResultTypes { get; set; }

        public bool ExcludedRedirect { get; set; }
        public string ExcludedRedirectMessage { get; set; }
        public string ExcludedRedirectUrl { get; set; }

        public bool DuplicateOfFirst { get; set; }
        public string DuplicateOfFirstMessage { get; set; }
        public string DuplicateOfFirstUrl { get; set; }

        public bool DuplicateOfLast { get; set; }
        public string DuplicateOfLastMessage { get; set; }
        public string DuplicateOfLastUrl { get; set; }
    }
}