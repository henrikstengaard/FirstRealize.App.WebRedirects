using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
	public class UrlHelper : IUrlHelper
    {
        private readonly IConfiguration _configuration;
		private readonly IUrlParser _urlParser;
		private readonly IUrlFormatter _urlFormatter;

        public UrlHelper(
            IConfiguration configuration,
			IUrlParser urlParser,
			IUrlFormatter urlFormatter)
        {
            _configuration = configuration;
			_urlParser = urlParser;
			_urlFormatter = urlFormatter;
        }

		public string Combine(
			string url1,
			string url2)
		{
			var url1Parsed = _urlParser.Parse(url1);

			if (!url1Parsed.IsValid)
			{
				throw new UriFormatException(
					string.Format(
						"Url '{0}' format is not valid",
						url1));
			}

			var url2Parsed = _urlParser.Parse(url2);

			if (!url2Parsed.IsValid)
			{
				throw new UriFormatException(
					string.Format(
						"Url '{0}' format is not valid",
						url2));
			}

			return _urlFormatter.Format(
				new ParsedUrl
				{
					Scheme = url1Parsed.Scheme,
					Host = url1Parsed.Host,
					Port = url1Parsed.Port,
					PathAndQuery = url2Parsed.PathAndQuery
				});
		}

		public bool IsHttpsRedirect(
            string oldUrl,
            string newUrl)
        {
            if (string.IsNullOrWhiteSpace(oldUrl) ||
                string.IsNullOrWhiteSpace(newUrl))
            {
                return false;
            }

            var oldUrlHttpsScheme = Regex.Replace(
                oldUrl,
                "^https?://",
                "https://",
                RegexOptions.IgnoreCase | RegexOptions.Compiled).ToLower();
            return oldUrlHttpsScheme.Equals(
                newUrl);
        }

        public string FormatUrl(
            string url)
        {
			if (string.IsNullOrWhiteSpace(url) ||
				!Regex.IsMatch(
					url,
					"^https?://",
					RegexOptions.IgnoreCase | RegexOptions.Compiled))
			{
				return url;
			}

			var uri = new Uri(url);

            var forceHttpUrlPatternMatch =
				uri != null &&
                _configuration.ForceHttpHostPatterns.Any(x => Regex.IsMatch(
					uri.DnsSafeHost,
                    x, RegexOptions.IgnoreCase | RegexOptions.Compiled) || Regex.IsMatch(
					uri.Host,
					x, RegexOptions.IgnoreCase | RegexOptions.Compiled));

            return forceHttpUrlPatternMatch
                ? Regex.Replace(
                    url,
                    "^https?://",
                    "http://",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled)
                : url;
        }

   //     public bool AreIdentical(
   //         IUrl url1,
   //         IUrl url2)
   //     {
   //         if (url1 == null ||
   //             !url1.IsValid ||
   //             url2 == null ||
   //             !url2.IsValid)
   //         {
   //             return false;
   //         }

			//return AreIdentical(
			//	url1.Parsed.AbsoluteUri,
			//	url2.Parsed.AbsoluteUri);
   //     }

		public bool AreIdentical(
			string url1,
			string url2)
		{
			if (string.IsNullOrWhiteSpace(url1) ||
				string.IsNullOrWhiteSpace(url2))
			{
				return false;
			}

			var url1Formatted = FormatUrl(url1);
			var url2Formatted = FormatUrl(url2);

			return url1Formatted.Equals(
				url2Formatted);
		}
	}
}