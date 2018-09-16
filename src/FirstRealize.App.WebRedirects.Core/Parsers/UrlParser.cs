using System;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class UrlParser
    {
        public Uri ParseUrl(string url, Uri host = null)
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
                return new Uri(url);
            }

            // return uri of host combined with url, if url starts with "/" and host is defined
            if (url.StartsWith("/") && host != null)
            {
                return new Uri(host, url);
            }

            var domainMatch = Regex.Match(url, "^[a-z0-9]+\\.[a-z0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (domainMatch.Success)
            {
                return new Uri(
                    string.Format("{0}://{1}",
                    host != null ? host.Scheme : "http",
                    url));
            }

            // return null as url is not a valid
            return null;
        }
    }
}