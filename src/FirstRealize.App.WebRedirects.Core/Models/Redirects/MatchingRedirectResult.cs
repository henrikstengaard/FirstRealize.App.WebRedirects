namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
	public class MatchingRedirectResult
	{
		public bool HasMatch { get;set; }
		public RedirectType ResultRedirectType { get; set; }
		public IParsedRedirect ParsedRedirect { get; set; }
	}
}