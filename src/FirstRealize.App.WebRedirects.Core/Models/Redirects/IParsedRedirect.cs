namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IParsedRedirect
    {
        IUrl OldUrl { get; }
        IUrl NewUrl { get; }
        bool IsValid { get; }
        bool IsIdentical { get; }
    }
}