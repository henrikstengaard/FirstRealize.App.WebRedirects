using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
	public class RedirectHelper : IRedirectHelper
	{
		private readonly IConfiguration _configuration;
		private readonly IUrlParser _urlParser;
		private readonly IUrlFormatter _urlFormatter;

		public RedirectHelper(
			IConfiguration configuration,
			IUrlParser urlParser,
			IUrlFormatter urlFormatter)
		{
			_configuration = configuration;
			_urlParser = urlParser;
			_urlFormatter = urlFormatter;
		}

		public string Replace(string url, IParsedRedirect parsedRedirect)
		{
			var parsedUrl = _urlParser.Parse(
				url,
				_configuration.DefaultUrl);

			var oldUrlSegments = parsedUrl.Path.Split('/');
			var newUrlSegments = parsedRedirect.NewUrl.Parsed.Path.Split('/');

			var replacedUrlSegments = newUrlSegments.Concat(
				oldUrlSegments
				.Skip(newUrlSegments.Length)
				.Take(oldUrlSegments.Length - newUrlSegments.Length));

			var replacedPath = string.Join(
				"/",
				replacedUrlSegments);

			parsedUrl.Path = replacedPath;

			return _urlFormatter.Format(parsedUrl);
		}
	}
}