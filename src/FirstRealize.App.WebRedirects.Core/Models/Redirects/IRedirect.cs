namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IRedirect
    {
        string OldUrl { get; set; }
        string NewUrl { get; set; }
        bool OldUrlHasHost { get; set; }
        bool NewUrlHasHost { get; set; }
        string ParsedOldUrl { get; set; }
        string ParsedNewUrl { get; set; }
        string OriginalOldUrl { get; set; }
        bool OriginalOldUrlHasHost { get; set; }
        string OriginalNewUrl { get; set; }
        bool OriginalNewUrlHasHost { get; set; }
		RedirectType RedirectType { get; set; }
	}
}