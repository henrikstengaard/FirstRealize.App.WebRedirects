namespace FirstRealize.App.WebRedirects.Core.Models.Results
{
    public static class ResultTypes
    {
        public static string InvalidResult = "InvalidResult";
        public static string IdenticalResult = "IdenticalResult";
        public static string ExcludedRedirect = "ExcludedRedirect";
        public static string DuplicateOfFirst = "DuplicateOfFirst";
        public static string DuplicateOfLast = "DuplicateOfLast";
        public static string CyclicRedirect = "CyclicRedirect";
        public static string TooManyRedirects = "TooManyRedirects";
        public static string OptimizedRedirect = "OptimizedRedirect";
        public static string UrlResponse = "UrlResponse";
    }
}