using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
    public class UrlHelper : IUrlHelper
    {
        private readonly IConfiguration _configuration;

        public UrlHelper(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsHttpsRedirect(
            IUrl oldUrl,
            IUrl newUrl)
        {
            if (oldUrl == null ||
                !oldUrl.IsValid ||
                newUrl == null ||
                !newUrl.IsValid)
            {
                return false;
            }

            var oldUrlHttpsScheme = Regex.Replace(
                oldUrl.Parsed.AbsoluteUri,
                "^https?://",
                "https://",
                RegexOptions.IgnoreCase | RegexOptions.Compiled).ToLower();
            return oldUrlHttpsScheme.Equals(
                newUrl.Parsed.AbsoluteUri);
        }

        public string FormatUrl(
            Uri parsedUrl)
        {
            var forceHttpUrlPatternMatches =
                _configuration.ForceHttpHostPatterns.Where(x => Regex.IsMatch(
                    parsedUrl.DnsSafeHost,
                    x, RegexOptions.IgnoreCase | RegexOptions.Compiled));

            return forceHttpUrlPatternMatches.Any()
                ? Regex.Replace(
                    parsedUrl.AbsoluteUri,
                    "^https?://",
                    "http://",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled)
                : parsedUrl.AbsoluteUri;
        }

        public bool AreIdentical(
            IUrl url1,
            IUrl url2)
        {
            if (url1 == null ||
                !url1.IsValid || 
                url2 == null ||
                !url2.IsValid)
            {
                return false;
            }

            return url1.Parsed.AbsoluteUri.Equals(
                url2.Parsed.AbsoluteUri);
        }
    }
}