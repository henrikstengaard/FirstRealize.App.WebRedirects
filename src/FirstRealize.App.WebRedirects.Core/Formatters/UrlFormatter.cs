using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System;

namespace FirstRealize.App.WebRedirects.Core.Formatters
{
	public class UrlFormatter : IUrlFormatter
	{
		public string Format(IParsedUrl parsedUrl)
		{
			if (parsedUrl == null)
			{
				throw new ArgumentNullException(nameof(parsedUrl));
			}

			if (!parsedUrl.IsValid)
			{
				throw new UriFormatException(
					"Url format is not valid");
			}

			return string.Format(
				"{0}://{1}{2}{3}",
				parsedUrl.Scheme,
				parsedUrl.Host,
				(parsedUrl.Scheme.ToLower().Equals("https") && parsedUrl.Port != 443) ||
				(parsedUrl.Scheme.ToLower().Equals("http") && parsedUrl.Port != 80)
				? string.Format(":{0}", parsedUrl.Port) : string.Empty,
				parsedUrl.PathAndQuery);
		}
	}
}