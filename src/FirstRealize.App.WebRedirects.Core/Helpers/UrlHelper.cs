using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
    public class UrlHelper : IUrlHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;
        private readonly IUrlFormatter _urlFormatter;

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

            var uri = new Uri(url);

            var forceHttpUrlPatternMatch =
                _forceHttpHostRegexs.Any(x => x.IsMatch(uri.DnsSafeHost)) ||
                _forceHttpHostRegexs.Any(x => x.IsMatch(uri.Host));

            return forceHttpUrlPatternMatch
                ? _schemeRegex.Replace(url, "http://")
                : url;
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
    }
}