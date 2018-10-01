using FirstRealize.App.WebRedirects.Core.Models.Urls;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IUrlParser
    {
		IParsedUrl Parse(
			string url,
			IParsedUrl defaultUrl = null,
			bool stripFragment = false);
    }
}