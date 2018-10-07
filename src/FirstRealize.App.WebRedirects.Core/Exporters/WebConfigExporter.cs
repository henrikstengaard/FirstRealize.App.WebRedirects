using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Extensions;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public class WebConfigExporter : IExporter
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;
        private readonly IUrlFormatter _urlFormatter;
        private readonly Regex _headingSlashRegex;
        private readonly Regex _tailingSlashRegex;

        private readonly string _webConfigTemplate = @"<configuration>
    <system.web>
        <customErrors mode=""Off"" />
    </system.web>
    <system.webServer >
        <rewrite>
            <rules>
{0}
            </rules>
            <rewriteMaps>
{1}
            </rewriteMaps>
        </rewrite>
    </system.webServer>
</configuration>";

        private readonly string _rewriteRuleTemplate = @"<rule name=""{0}"" stopProcessing=""true"">
    <match url=""{1}"" />
{2}
    <action type=""Redirect"" url=""{3}"" redirectType=""Permanent"" appendQueryString=""False"" />
</rule>";

        private readonly string _rewriteRuleConditionsTemplate = @"<conditions>
{0}
</conditions>";

        private readonly string _rewriteMapTemplate = @"<rewriteMap name=""{0}"" defaultValue="""">
{1}
</rewriteMap>";

        public WebConfigExporter(
            IConfiguration configuration,
            IUrlParser urlParser,
            IUrlFormatter urlFormatter)
        {
            _configuration = configuration;
            _urlParser = urlParser;
            _urlFormatter = urlFormatter;
            _headingSlashRegex = new Regex(
                "^[/]+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _tailingSlashRegex = new Regex(
                "[/]+$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public string Name => "WebConfig";

        public string Build(
            IEnumerable<IRedirect> redirects)
        {
            // build top rewrites index
            var topRewritesIndex = new Dictionary<string, Rewrite>();
            foreach (var rewrite in BuildRewrites(
                redirects.ToList()))
            {
                if (!topRewritesIndex.ContainsKey(rewrite.Id))
                {
                    topRewritesIndex.Add(rewrite.Id, rewrite);
                }

                topRewritesIndex[rewrite.Id].RelatedRewrites.Add(rewrite);
            }

            // sort top rewrites
            var topRewritesSorted = topRewritesIndex.Keys
                .Where(x => !string.IsNullOrWhiteSpace(topRewritesIndex[x].OldUrl.Query))
                .OrderByDescending(x => topRewritesIndex[x].OldUrl.PathAndQuery)
                .OrderByDescending(x => topRewritesIndex[x].OldUrlHasHost)
                .OrderByDescending(x => topRewritesIndex[x].NewUrlHasHost)
                .Concat(topRewritesIndex.Keys
                .Where(x => string.IsNullOrWhiteSpace(topRewritesIndex[x].OldUrl.Query))
                .OrderByDescending(x => topRewritesIndex[x].OldUrl.PathAndQuery)
                .OrderByDescending(x => topRewritesIndex[x].OldUrlHasHost)
                .OrderByDescending(x => topRewritesIndex[x].NewUrlHasHost))
                .Select(x => topRewritesIndex[x])
                .ToList();

            // build rewrite rules and maps
            var rewriteRules = new List<string>();
            var rewriteMaps = new List<string>();
            foreach (var topRewrite in topRewritesSorted)
            {
                // match url
                var matchUrl = topRewrite.RelatedRewrites.Count <= 1
                    ? string.Format("^{0}/?$", FormatOldPath(topRewrite.OldUrl.Path))
                    : topRewrite.OldUrlHasRootPath
                    ? "^/?$"
                    : "^(.+?)/?$";

                // conditions
                var conditions = BuildConditions(topRewrite);
                var conditionsFormatted = conditions.Any()
                    ? string.Format(
                        _rewriteRuleConditionsTemplate,
                        string.Join(Environment.NewLine, conditions))
                    : string.Empty;

                // redirect url
                var redirectUrl = topRewrite.RelatedRewrites.Count <= 1
                    ? XmlEncode(topRewrite.NewUrl.OriginalUrl)
                    : topRewrite.NewUrlHasHost
                    ? string.Format("{0}://{1}{{C:1}}", topRewrite.NewUrl.Scheme, topRewrite.NewUrl.Host)
                    : "{C:1}";

                // build rewrite rule
                rewriteRules.Add(string.Format(
                    _rewriteRuleTemplate,
                    XmlEncode(topRewrite.Name),
                    matchUrl,
                    conditionsFormatted,
                    redirectUrl));

                // skip rewrite maps for top rewrites with only 1 related rewrite
                if (topRewrite.RelatedRewrites.Count <= 1)
                {
                    continue;
                }

                // build rewrite map
                rewriteMaps.Add(
                    string.Format(
                        _rewriteMapTemplate,
                        topRewrite.Id,
                        string.Join(Environment.NewLine, topRewrite.RelatedRewrites.OrderByDescending(x => x.OldUrl.PathAndQuery).Select(x => string.Format(
                                "<add key=\"{0}\" value=\"{1}\" />",
                                XmlEncode(FormatOldPath(x.OldUrl.Path)),
                                XmlEncode(FormatNewPathAndQuery(x.NewUrl.Path, x.NewUrl.Query))
                                )))));
            }

            return string.Format(
                _webConfigTemplate,
                string.Join(Environment.NewLine, rewriteRules),
                string.Join(Environment.NewLine, rewriteMaps));
        }

        private IEnumerable<Rewrite> BuildRewrites(
            IEnumerable<IRedirect> redirects)
        {
            foreach (var redirect in redirects.ToList())
            {
                var oldUrlRefined = _urlParser.Parse(
                    redirect.OldUrl,
                    _configuration.DefaultUrl);
                var newUrlRefined = _urlParser.Parse(
                    redirect.NewUrl,
                    _configuration.DefaultUrl);

                var oldUrlHasRootPath = oldUrlRefined.Path.Equals("/");

                if (_configuration.ExcludeOldUrlRootRedirects && oldUrlHasRootPath)
                {
                    continue;
                }

                var rewriteKey = BuildRewriteKey(
                    redirect.OldUrlHasHost,
                    oldUrlRefined.Query,
                    oldUrlHasRootPath,
                    redirect.NewUrlHasHost,
                    newUrlRefined.Host);

                var rewriteName = BuildRewriteName(
                    redirect.OldUrlHasHost,
                    oldUrlRefined.Host,
                    oldUrlRefined.Query,
                    oldUrlHasRootPath,
                    redirect.NewUrlHasHost,
                    newUrlRefined.Host);

                yield return new Rewrite
                {
                    Id = rewriteKey.ToMd5().ToLower(),
                    Name = rewriteName,
                    OldUrlHasRootPath = oldUrlHasRootPath,
                    OldUrlHasHost = redirect.OldUrlHasHost,
                    NewUrlHasHost = redirect.NewUrlHasHost,
                    OldUrl = oldUrlRefined,
                    NewUrl = newUrlRefined
                };
            }
        }

        private string BuildRewriteName(
            bool oldUrlHasHost,
            string oldUrlHost,
            string oldUrlQueryString,
            bool oldUrlHasRootRedirect,
            bool newUrlHasHost,
            string newUrlHost)
        {
            return string.Format(
                "Rewrite rule for {0}{1}{2}{3}",
                oldUrlHasRootRedirect ? "root url " : "urls ",
                oldUrlHasHost
                ? string.Format("from host '{0}'", oldUrlHost)
                : "from any host",
                !string.IsNullOrWhiteSpace(oldUrlQueryString)
                ? string.Format(" with query string '{0}'", oldUrlQueryString)
                : string.Empty,
                newUrlHasHost
                ? string.Format(" to host '{0}'", newUrlHost)
                : " to same host");
        }

        private string BuildRewriteKey(
            bool oldUrlHasHost,
            string oldUrlQueryString,
            bool oldUrlHasRootRedirect,
            bool newUrlHasHost,
            string newUrlHost)
        {
            return string.Format(
                "OldUrlHasHost={0}{1}{2}|NewUrlHasHost={3}{4}",
                oldUrlHasHost,
                oldUrlHasRootRedirect ? ",/" : string.Empty,
                !string.IsNullOrWhiteSpace(oldUrlQueryString)
                ? string.Format(",{0}", oldUrlQueryString)
                : string.Empty,
                newUrlHasHost,
                newUrlHasHost
                ? string.Format(",{0}", newUrlHost.ToLower())
                : string.Empty);
        }

        public void Export(
            IEnumerable<IRedirect> redirects,
            string outputDir)
        {
            var webConfigFile = Path.Combine(
                outputDir,
                "web.config");

            File.WriteAllText(
                webConfigFile,
                Build(redirects));
        }

        public IEnumerable<string> BuildConditions(
            Rewrite rewrite)
        {
            if (rewrite.OldUrlHasHost &&
                !string.IsNullOrEmpty(rewrite.OldUrl.Host))
            {
                yield return string.Format(
                    "<add input=\"{{HTTP_HOST}}\" pattern=\"^{0}$\" />",
                    rewrite.OldUrl.Host);
            }

            if (!string.IsNullOrEmpty(rewrite.OldUrl.Query))
            {
                yield return string.Format(
                    "<add input=\"{{QUERY_STRING}}\" pattern=\"{0}\" />",
                    XmlEncode(Regex.Escape(rewrite.OldUrl.Query)));
            }

            if (rewrite.RelatedRewrites.Count > 1)
            {
                yield return string.Format(
                    "<add input=\"{{{0}:{{R:1}}}}\" pattern=\"(.+)\" />",
                    rewrite.Id);
            }
        }

        private string FormatNewPathAndQuery(
            string path,
            string query)
        {
            var formatted = path;

            if (string.IsNullOrWhiteSpace(query))
            {
                return formatted;
            }

            var parameters =
                query.Split('&')
             .ToDictionary(c => c.Split('=')[0],
                           c => Uri.UnescapeDataString(c.Split('=')[1]),
                           StringComparer.OrdinalIgnoreCase);

            return string.Format("{0}?{1}",
                formatted,
                string.Join(
                    "&",
                    parameters.Select(
                        p => string.Format("{0}={1}", p.Key, Uri.EscapeDataString(p.Value)))));
        }

        private string FormatOldPath(
            string path)
        {
            return _tailingSlashRegex.Replace(
                _headingSlashRegex.Replace(
                    path,
                    string.Empty),
                string.Empty);
        }

        private string XmlEncode(
            string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}