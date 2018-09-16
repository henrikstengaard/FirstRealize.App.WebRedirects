namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class ProcessedRedirect
    {
        public Redirect Redirect { get; set; }
        public bool IsExcluded { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsCyclic { get; set; }
        public bool HitRedirectLimit { get; set; }
        public string Status { get; set; }
    }
}