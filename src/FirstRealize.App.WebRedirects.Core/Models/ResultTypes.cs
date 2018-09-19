namespace FirstRealize.App.WebRedirects.Core.Models
{
    public static class ResultTypes
    {
        public static string ExcludedRedirect = "ExcludedRedirect";
        public static string DuplicateOfFirst = "DuplicateOfFirst";
        public static string DuplicateOfLast = "DuplicateOfLast";
        public static string CyclicRedirect = "CyclicRedirect";
        public static string OptimizedRedirect = "OptimizedRedirect";
        public static string UrlWithResponse = "UrlWithResponse";
    }
}