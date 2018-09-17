namespace FirstRealize.App.WebRedirects.Core.Models
{
    public interface IResult
    {
        string Type { get; }
        string Message { get; }
    }
}