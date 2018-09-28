namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IRedirect
    {
        string OldUrl { get; set; }
        string NewUrl { get; set; }
        bool OldUrlHasHost { get; set; }
        bool NewUrlHasHost { get; set; }
        string OldUrlParsed { get; set; }
        string NewUrlParsed { get; set; }
        string OldUrlOriginal { get; set; }
        string NewUrlOriginal { get; set; }
    }
}