using System;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class UrlParser : IUrlParser
    {
        public Uri ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            // remove whitespaces
            url = Regex.Replace(url, "\\s+", "", RegexOptions.Compiled);

            // return uri of url, if url starts with "http://" or "https://"
            if (Regex.IsMatch(url, "https?://", RegexOptions.Compiled))
            {
                return FormatUri(
                    new Uri(url),
                    stripFragment);
            }

            // return uri of host combined with url, if url starts with "/" and host is defined
            if (url.StartsWith("/") && host != null)
            {
                return FormatUri(
                    new Uri(host, url),
                    stripFragment);
            }

            var domainMatch = Regex.Match(url, "^[a-z0-9]+\\.[a-z0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (domainMatch.Success)
            {
                return FormatUri(new Uri(
                    string.Format("{0}://{1}",
                    host != null ? host.Scheme : "http",
                    url)),
                    stripFragment);
            }

            // return null as url is not a valid
            return null;
        }

        private Uri FormatUri(
            Uri uri,
            bool stripFragment = false)
        {
            UriBuilder builder = new UriBuilder(uri);

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