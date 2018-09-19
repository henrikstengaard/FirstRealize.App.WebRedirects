namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class Result : IResult
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public Url Url { get; set; }
    }
}