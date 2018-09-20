using FirstRealize.App.WebRedirects.Core.Models;
using System;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class UrlParser : IUrlParser
    {
        public Url ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            // remove whitespaces
            var formattedUrl = Regex.Replace(
                url,
                "\\s+", "",
                RegexOptions.Compiled);

            // return uri of url, if url starts with "http://" or "https://"
            if (Regex.IsMatch(
                formattedUrl,
                "https?://",
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

            var domainMatch = Regex.Match(
                formattedUrl, 
                "^[a-z0-9]+\\.[a-z0-9]+", 
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (domainMatch.Success)
            {
                return new Url
                {
                    Raw = url,
                    Parsed = FormatUri(new Uri(
                        string.Format("{0}://{1}",
                        host != null ? host.Scheme : "http",
                        formattedUrl)),
                        stripFragment),
                    HasHost = true
                };
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