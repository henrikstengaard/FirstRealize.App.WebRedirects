using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class UrlParser : IUrlParser
    {
        private readonly IConfiguration _configuration;

        public UrlParser(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IParsedUrl Parse(
            string url,
            IParsedUrl defaultUrl = null,
            bool stripFragment = false)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            // remove whitespaces fro murl
            var urlFormatted = Regex.Replace(
                url,
                "\\s+", "",
                RegexOptions.Compiled);

            // match url scheme
            var urlSchemeMatch = Regex.Match(
                urlFormatted,
                "^(http|https)://([^/]+):?([^/]*)(.*)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // return parsed url, if url matches http or https scheme
            if (urlSchemeMatch.Success)
            {
                var scheme = urlSchemeMatch.Groups[1].Value.ToLower();

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
                    Host = defaultUrl != null && !string.IsNullOrWhiteSpace(defaultUrl.Host)
                        ? defaultUrl.Host
                        : urlSchemeMatch.Groups[2].Value,
                    Port = port,
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

            // return parsed url with default url, if it starts with '/'
            if (urlFormatted.StartsWith("/"))
            {
                var pathAndQuery = FormatPathAndQuery(
                        urlFormatted,
                        stripFragment);

                var pathAndQueryParts = pathAndQuery.Split(new[] { '?' });

                return new ParsedUrl
                {
                    Scheme = defaultUrl != null && defaultUrl.IsValid
                        ? defaultUrl.Scheme
                        : _configuration.DefaultUrl.Scheme,
                    Port = defaultUrl != null && defaultUrl.IsValid
                        ? defaultUrl.Port
                        : _configuration.DefaultUrl.Port,
                    Host = defaultUrl != null && defaultUrl.IsValid
                        ? defaultUrl.Host
                        : _configuration.DefaultUrl.Host,
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
                OriginalUrl = urlFormatted
            };
        }

        private int ParsePort(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            int port;
            if (!int.TryParse(value, out port))
            {
                throw new HttpException(
                    string.Format(
                        "Invalid port '{0}'",
                        value));
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
                pathAndQuery = Regex.Replace(
                    pathAndQuery,
                    "#[^#\\?]*",
                    string.Empty,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return pathAndQuery;
        }

        public IUrl ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false)
        {
            // return url with only raw url, if url is not defined
            if (string.IsNullOrWhiteSpace(url))
            {
                return new Url
                {
                    Raw = url
                };
            }

            // remove whitespaces
            var formattedUrl = Regex.Replace(
                url,
                "\\s+", "",
                RegexOptions.Compiled);

            // return uri of url, if url starts with "http://" or "https://"
            if (Regex.IsMatch(
                formattedUrl,
                "^https?://",
                RegexOptions.Compiled))
            {
                return new Url
                {
                    Raw = url,
                    Parsed = FormatUri(
                        new Uri(formattedUrl),
                        stripFragment),
                    HasHost = true
                };
            }

            // return uri of host combined with url, if url starts with "/" and host is defined
            if (formattedUrl.StartsWith("/") && host != null)
            {
                return new Url
                {
                    Raw = url,
                    Parsed = FormatUri(
                        new Uri(host, formattedUrl),
                        stripFragment)
                };
            }

            // return url with only raw url
            return new Url
            {
                Raw = url
            };
        }

        private Uri FormatUri(
            Uri uri,
            bool stripFragment = false)
        {
            var builder = new UriBuilder(uri);

            // change scheme, if host match default url
            if (uri.Host == _configuration.DefaultUrl.Host &&
                uri.Scheme != _configuration.DefaultUrl.Scheme)
            {
                builder.Scheme = _configuration.DefaultUrl.Scheme;
            }

            // remove tailing slash, if path ends with slash
            if (uri.AbsolutePath.EndsWith("/"))
            {
                builder.Path = Regex.Replace(
                    builder.Path,
                    "/+$",
                    string.Empty,
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            if (stripFragment && !string.IsNullOrEmpty(builder.Fragment))
            {
                builder.Fragment = string.Empty;
            }

            return builder.Uri;
        }
    }
}