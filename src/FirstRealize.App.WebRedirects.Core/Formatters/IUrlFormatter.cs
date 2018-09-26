using FirstRealize.App.WebRedirects.Core.Models.Urls;

namespace FirstRealize.App.WebRedirects.Core.Formatters
{
	public interface IUrlFormatter
	{
		string Format(
			IParsedUrl parsedUrl);
	}
}