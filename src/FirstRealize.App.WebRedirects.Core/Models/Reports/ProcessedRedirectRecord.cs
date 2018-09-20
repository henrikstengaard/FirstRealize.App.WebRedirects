namespace FirstRealize.App.WebRedirects.Core.Models.Reports
{
    public class ProcessedRedirectRecord
    {
        public string OldUrlRaw { get; set; }
        public string NewUrlRaw { get; set; }
        public string OldUrlParsed { get; set; }
        public string NewUrlParsed { get; set; }

        public int ResultCount { get; set; }
        public string ResultTypes { get; set; }

        public bool InvalidRedirect { get; set; }
        public string InvalidRedirectMessage { get; set; }

        public bool IdenticalRedirect { get; set; }
        public string IdenticalRedirectMessage { get; set; }

        public bool ExcludedRedirect { get; set; }
        public string ExcludedRedirectMessage { get; set; }
        public string ExcludedRedirectUrl { get; set; }

        public bool DuplicateOfFirst { get; set; }
        public string DuplicateOfFirstMessage { get; set; }
        public string DuplicateOfFirstUrl { get; set; }

        public bool DuplicateOfLast { get; set; }
        public string DuplicateOfLastMessage { get; set; }
        public string DuplicateOfLastUrl { get; set; }

        public bool UrlResponse { get; set; }
        public string UrlResponseMessage { get; set; }
        public string UrlResponseUrl { get; set; }
        public int UrlResponseStatusCode { get; set; }
        public string UrlResponseLocation { get; set; }

        public bool OptimizedRedirect { get; set; }
        public string OptimizedRedirectMessage { get; set; }
        public string OptimizedRedirectUrl { get; set; }
        public int OptimizedRedirectCount { get; set; }

        public bool CyclicRedirect { get; set; }
        public string CyclicRedirectMessage { get; set; }
        public string CyclicRedirectUrl { get; set; }
        public int CyclicRedirectCount { get; set; }

        public bool TooManyRedirects { get; set; }
        public string TooManyRedirectsMessage { get; set; }
        public string TooManyRedirectsUrl { get; set; }
        public int TooManyRedirectsCount { get; set; }

        public bool UnknownError { get; set; }
        public string UnknownErrorMessage { get; set; }
    }
}