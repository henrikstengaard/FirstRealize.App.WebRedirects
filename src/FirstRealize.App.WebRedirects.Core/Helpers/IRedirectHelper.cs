using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
	public interface IRedirectHelper
	{
		string Replace(
			string url,
			IParsedRedirect parsedRedirect);
	}
}