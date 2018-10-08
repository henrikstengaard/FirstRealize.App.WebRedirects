using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class UrlParser : IUrlParser
    {
        private readonly Regex _formatUrlRegex;
        private readonly Regex _urlRegex;
        private readonly Regex _portRegex;
        private readonly Regex _fragmentRegex;

        public UrlParser()
        {
            _formatUrlRegex = new Regex(
                "\\s+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _urlRegex = new Regex(
                "^(http|https)://([^:/]+):?([^:/]*)(.*)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _portRegex = new Regex(
                "^\\d+$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _fragmentRegex = new Regex(
                "#[^#\\?]*",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public IParsedUrl Parse(
            string url,
            IParsedUrl defaultUrl,
            bool stripFragment = false)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            // remove whitespaces fro murl
            var urlFormatted = _formatUrlRegex.Replace(
                url,
                string.Empty);

            // match url scheme
            var urlSchemeMatch = _urlRegex.Match(
                urlFormatted);

            // return parsed url, if url matches http or https scheme
            if (urlSchemeMatch.Success)
            {
                var scheme = urlSchemeMatch.Groups[1].Value.ToLower();

                if (!string.IsNullOrWhiteSpace(urlSchemeMatch.Groups[3].Value) &&
                    !_portRegex.IsMatch(urlSchemeMatch.Groups[3].Value))
                {
                    return new ParsedUrl
                    {
                        OriginalUrl = url
                    };
                }

                var port = ParsePort(
                    urlSchemeMatch.Groups[3].Value);

                if (port == 0)
                {
                    port = scheme.ToLower().StartsWith("https")
                        ? 443
                        : 80;
                }

                var pathAndQuery = FormatPathAndQuery(
                        urlSchemeMatch.Groups[4].Value,
                        stripFragment);

                var pathAndQueryParts = pathAndQuery.Split(new[] { '?' });

                return new ParsedUrl
                {
                    Scheme = scheme,
                    Host = urlSchemeMatch.Groups[2].Value,
                    Port = port,
                    PathAndQuery = pathAndQuery,
                    Path = pathAndQueryParts.Length > 0
                        ? pathAndQueryParts[0]
                        : pathAndQuery,
                    Query = pathAndQueryParts.Length > 1
                        ? pathAndQueryParts[1]
                        : string.Empty,
                    OriginalUrl = urlFormatted,
                    OriginalUrlHasHost = true
                };
            }

            // return parsed url with default url, if it starts with '/'
            if (urlFormatted.StartsWith("/"))
            {
                if (defaultUrl == null ||
                    !defaultUrl.IsValid)
                {
                    return new ParsedUrl
                    {
                        OriginalUrl = url
                    };
                }

                var pathAndQuery = FormatPathAndQuery(
                        urlFormatted,
                        stripFragment);

                var pathAndQueryParts = pathAndQuery.Split(new[] { '?' });

                return new ParsedUrl
                {
                    Scheme = defaultUrl.Scheme,
                    Port = defaultUrl.Port,
                    Host = defaultUrl.Host,
                    PathAndQuery = pathAndQuery,
                    Path = pathAndQueryParts.Length > 0
                        ? pathAndQueryParts[0]
                        : pathAndQuery,
                    Query = pathAndQueryParts.Length > 1
                        ? pathAndQueryParts[1]
                        : string.Empty,
                    OriginalUrl = urlFormatted
                };
            }

            return new ParsedUrl
            {
                OriginalUrl = url
            };
        }

        private int ParsePort(
            string value)
        {
            int port;
            if (string.IsNullOrWhiteSpace(value) ||
                !int.TryParse(value, out port))
            {
                return 0;
            }

            return port;
        }

        private string FormatPathAndQuery(
            string pathAndQuery,
            bool stripFragment = false)
        {
            pathAndQuery = !pathAndQuery.StartsWith("/")
                ? string.Concat("/", pathAndQuery)
                : pathAndQuery;

            if (stripFragment)
            {
                pathAndQuery = _fragmentRegex.Replace(
                    pathAndQuery,
                    string.Empty);
            }

            return pathAndQuery;
        }
    }
}