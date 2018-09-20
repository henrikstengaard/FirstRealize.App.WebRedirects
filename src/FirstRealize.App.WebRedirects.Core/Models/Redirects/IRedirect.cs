namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IRedirect
    {
        string OldUrl { get; set; }
        string NewUrl { get; set; }
    }
}