namespace FirstRealize.App.WebRedirects.Core.Helpers
{
	public interface IUrlHelper
    {
		string Combine(
			string url1,
			string url2);
        bool IsHttpsRedirect(
            string oldUrl,
            string newUrl);
        string FormatUrl(
            string url);
		bool AreIdentical(
            string url1,
            string url2);
	}
}