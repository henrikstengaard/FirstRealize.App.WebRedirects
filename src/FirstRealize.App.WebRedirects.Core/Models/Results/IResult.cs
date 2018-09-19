namespace FirstRealize.App.WebRedirects.Core.Models.Results
{
    public interface IResult
    {
        string Type { get; }
        string Message { get; }
        Url Url { get; }
    }
}