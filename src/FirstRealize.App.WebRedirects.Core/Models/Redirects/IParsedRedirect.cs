namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IParsedRedirect
    {
        Url OldUrl { get; }
        Url NewUrl { get; }
        bool IsValid { get; }
        bool IsIdentical { get; }
    }
}