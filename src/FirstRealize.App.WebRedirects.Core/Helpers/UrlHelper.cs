using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
    public class UrlHelper : IUrlHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;
        private readonly IUrlFormatter _urlFormatter;

		private readonly IdnMapping _idnMapping;
        private readonly Regex _schemeRegex;
        private readonly IList<Regex> _forceHttpHostRegexs;

        public UrlHelper(
            IConfiguration configuration,
            IUrlParser urlParser,
            IUrlFormatter urlFormatter)
        {
            _configuration = configuration;
            _urlParser = urlParser;
            _urlFormatter = urlFormatter;

			_idnMapping = new IdnMapping();
            _schemeRegex = new Regex(
                    "^https?://",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _forceHttpHostRegexs = _configuration.ForceHttpHostPatterns
                .Select(x => new Regex(
                    x, RegexOptions.IgnoreCase | RegexOptions.Compiled))
                .ToList();
        }

        public string Combine(
            string url1,
            string url2)
        {
            var url1Parsed = _urlParser.Parse(
                url1,
                _configuration.DefaultUrl);

            if (!url1Parsed.IsValid)
            {
                throw new UriFormatException(
                    string.Format(
                        "Url '{0}' format is not valid",
                        url1));
            }

            var url2Parsed = _urlParser.Parse(
                url2,
                _configuration.DefaultUrl);

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
					Path = url2Parsed.Path,
					Query = url2Parsed.Query
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

            var oldUrlHttpsScheme =
                _schemeRegex.Replace(oldUrl, "https://");

            return oldUrlHttpsScheme.Equals(newUrl);
        }

        public string FormatUrl(
            string url)
        {
            if (string.IsNullOrWhiteSpace(url) ||
                !_schemeRegex.IsMatch(url))
            {
                return url;
            }

			var parsedUrl = 
				_urlParser.Parse(
					url,
					_configuration.DefaultUrl);

			var dnsSafeHost = 
				new IdnMapping().GetAscii(parsedUrl.Host);

            var forceHttpUrlPatternMatch =
                _forceHttpHostRegexs.Any(x => x.IsMatch(dnsSafeHost)) ||
                _forceHttpHostRegexs.Any(x => x.IsMatch(parsedUrl.Host));

            if (forceHttpUrlPatternMatch)
			{
				parsedUrl.Scheme = "http";
				parsedUrl.Port = 80;
			}

			return _urlFormatter.Format(parsedUrl);
        }

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

		public string GetParentPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return null;
			}

			if (path.LastIndexOf("/", 
				StringComparison.InvariantCultureIgnoreCase) == path.Length - 1)
			{
				path = path.Substring(0, path.Length - 1);
			}

			var lastSlashPosition = 
				path.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase);

			if (lastSlashPosition < 0)
			{
				return null;
			}

			return path.Substring(0, lastSlashPosition);
		}
    }
}