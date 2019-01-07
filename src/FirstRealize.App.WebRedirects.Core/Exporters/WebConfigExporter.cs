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
                // force exact redirect type, if top rewrite has query and has more than one related rewrite
                if (!string.IsNullOrWhiteSpace(topRewrite.OldUrl.Query) &&
                    topRewrite.RelatedRewrites.Count > 1)
                {
                    topRewrite.RedirectType = RedirectType.Exact;
                }

                var useRewriteMap =
                    topRewrite.RelatedRewrites.Count > 1;

                // match url
                string matchUrl =
                    XmlEncode(BuildMatchUrl(topRewrite));

                // conditions
                var conditions = BuildConditions(topRewrite);
                var conditionsFormatted = conditions.Any()
                    ? string.Format(
                        _rewriteRuleConditionsTemplate,
                        string.Join(Environment.NewLine, conditions))
                    : string.Empty;

                // redirect url
                var redirectUrl =
                    BuildRedirectUrl(topRewrite);

                // build rewrite rule
                rewriteRules.Add(string.Format(
                    _rewriteRuleTemplate,
                    XmlEncode(topRewrite.Name),
                    matchUrl,
                    conditionsFormatted,
                    redirectUrl));

                // skip rewrite maps for top rewrites with only 1 related rewrite
                if (!useRewriteMap)
                {
                    continue;
                }

                // build rewrite map index to ensure old url redirects are unique 
                var rewriteMapIndex = new Dictionary<string, Rewrite>();
                foreach (var relatedRewrite in topRewrite.RelatedRewrites)
                {
                    var key = string.IsNullOrWhiteSpace(relatedRewrite.OldUrl.Query)
                        ? FormatOldPath(relatedRewrite.OldUrl.Path)
                        : relatedRewrite.OldUrl.Query;
                    var value = FormatOldPath(relatedRewrite.NewUrl.Path);

                    if (key.ToLower().Equals(value.ToLower()))
                    {
                        continue;
                    }

                    if (rewriteMapIndex.ContainsKey(
                        key))
                    {
                        continue;
                    }

                    rewriteMapIndex.Add(
                        key,
                        relatedRewrite);
                }

                // build rewrite map
                rewriteMaps.Add(
                    string.Format(
                        _rewriteMapTemplate,
                        topRewrite.Id,
                        string.Join(
                            Environment.NewLine,
                            rewriteMapIndex.Values.OrderByDescending(x => x.OldUrl.PathAndQuery).Select(x => string.Format(
                                "<add key=\"{0}\" value=\"{1}\" />",
                                string.IsNullOrWhiteSpace(topRewrite.OldUrl.Query)
                                ? FormatOldPath(x.OldUrl.Path)
                                : x.OldUrl.Query,
                                FormatNewPathAndQuery(x.NewUrl.Path, x.NewUrl.Query))
                                ))));
            }

            return string.Format(
                _webConfigTemplate,
                string.Join(Environment.NewLine, rewriteRules),
                string.Join(Environment.NewLine, rewriteMaps));
        }

        private string BuildMatchUrl(Rewrite rewrite)
        {
            switch (rewrite.RedirectType)
            {
                case RedirectType.Exact:
                    return BuildExactMatchUrl(rewrite);
                case RedirectType.Replace:
                    return BuildReplaceMatchUrl(rewrite);
            }

            return ".+";
        }

        private string BuildExactMatchUrl(
            Rewrite rewrite)
        {
            var useRewriteMap =
                rewrite.RelatedRewrites.Count > 1;

            return rewrite.OldUrlHasRootPath
                ? "^/?$"
                : rewrite.RelatedRewrites.Count <= 1 || !string.IsNullOrWhiteSpace(rewrite.OldUrl.Query)
                    ? string.Format("^{0}/?$", FormatOldPath(rewrite.OldUrl.Path))
                    : "^(.+?)/?$";
        }

        private string BuildReplaceMatchUrl(
            Rewrite rewrite)
        {
            var useRewriteMap =
                rewrite.RelatedRewrites.Count > 1;
            var segments = rewrite.OldUrl.Path.Split('/').Length - 1;
            var path = rewrite.OldUrlHasRootPath
                ? string.Empty
                : FormatOldPath(rewrite.OldUrl.Path);

            return string.Format(
                "^{0}(.+)?/?$",
                useRewriteMap
                ? string.Format(
                    "({0})",
                    string.Join(
                        "/",
                        Enumerable.Repeat("[^/]+", segments)))
                : path);
        }

        private string BuildRedirectUrl(
            Rewrite rewrite)
        {
            var useRewriteMap =
                rewrite.RelatedRewrites.Count > 1;

            var host = rewrite.NewUrlHasHost
                ? string.Format("{0}://{1}", rewrite.NewUrl.Scheme, rewrite.NewUrl.Host)
                : "";

            var replaceRemainingPath =
                rewrite.RedirectType == RedirectType.Replace
                ? useRewriteMap ? "{R:2}" : "{R:1}"
                : string.Empty;

            var pathAndQuery = FormatNewPathAndQuery(
                string.Concat(
                    useRewriteMap ? "{C:1}" : rewrite.NewUrl.Path,
                    replaceRemainingPath),
                rewrite.NewUrl.Query);

            return string.Concat(
                host,
                pathAndQuery);
        }

        private IEnumerable<Rewrite> BuildRewrites(
            IEnumerable<IRedirect> redirects)
        {
            foreach (var redirect in redirects.ToList())
            {
                var oldUrlParsed = _urlParser.Parse(
                    redirect.OldUrl,
                    _configuration.DefaultUrl);
                var newUrlParsed = _urlParser.Parse(
                    redirect.NewUrl,
                    _configuration.DefaultUrl);

                var oldUrlHasRootPath = oldUrlParsed.Path.Equals("/");

                if (_configuration.ExcludeOldUrlRootRedirects && oldUrlHasRootPath)
                {
                    continue;
                }

                var segments = oldUrlParsed.Path.Split('/').Length - 1;

                var rewriteKey = BuildRewriteKey(
                    redirect.RedirectType,
                    segments,
                    redirect.OldUrlHasHost,
                    oldUrlParsed.Host,
                    oldUrlParsed.Path,
                    oldUrlParsed.Query,
                    oldUrlHasRootPath,
                    redirect.NewUrlHasHost,
                    newUrlParsed.Host);

                var rewriteName = BuildRewriteName(
                    redirect.RedirectType,
                    segments,
                    redirect.OldUrlHasHost,
                    oldUrlParsed.Host,
                    oldUrlParsed.Path,
                    oldUrlParsed.Query,
                    oldUrlHasRootPath,
                    redirect.NewUrlHasHost,
                    newUrlParsed.Host);

                yield return new Rewrite
                {
                    Id = rewriteKey.ToMd5().ToLower(),
                    Key = rewriteKey,
                    Name = rewriteName,
                    RedirectType = redirect.RedirectType,
                    OldUrlHasRootPath = oldUrlHasRootPath,
                    OldUrlHasHost = redirect.OldUrlHasHost,
                    NewUrlHasHost = redirect.NewUrlHasHost,
                    OldUrl = oldUrlParsed,
                    NewUrl = newUrlParsed
                };
            }
        }

        private string BuildRewriteName(
            RedirectType redirectType,
            int segments,
            bool oldUrlHasHost,
            string oldUrlHost,
            string oldUrlPath,
            string oldUrlQueryString,
            bool oldUrlHasRootRedirect,
            bool newUrlHasHost,
            string newUrlHost)
        {
            return string.Format(
                "{0}{1} rewrite rule for {2}{3}{4}{5}",
                redirectType,
                redirectType == RedirectType.Replace && segments > 0
                ? string.Format(" {0} segment(s)", segments)
                : string.Empty,
                oldUrlHasRootRedirect ? "root url " : "urls ",
                oldUrlHasHost
                ? string.Format("from host '{0}'", oldUrlHost)
                : "from any host",
                !string.IsNullOrWhiteSpace(oldUrlQueryString)
                ? string.Format(
                    " with {0}query string", oldUrlHasRootRedirect ? string.Empty : string.Format("path '{0}' and ", oldUrlPath))
                : string.Empty,
                newUrlHasHost
                ? string.Format(" to host '{0}'", newUrlHost)
                : " to same host");
        }

        private string BuildRewriteKey(
            RedirectType redirectType,
            int segments,
            bool oldUrlHasHost,
            string oldUrlHost,
            string oldUrlPath,
            string oldUrlQueryString,
            bool oldUrlHasRootRedirect,
            bool newUrlHasHost,
            string newUrlHost)
        {
            return string.Format(
                "{0}{1}OldUrlHasHost={2}{3}{4}{5}|NewUrlHasHost={6}{7}",
                redirectType,
                redirectType == RedirectType.Replace && segments > 0
                ? string.Format(",{0}Segments", segments)
                : string.Empty,
                oldUrlHasHost,
                oldUrlHasHost
                ? string.Format(",{0}", oldUrlHost.ToLower())
                : string.Empty,
                oldUrlHasRootRedirect ? ",/" : string.Empty,
                !string.IsNullOrWhiteSpace(oldUrlQueryString)
                ? string.Format(",{0}QueryString", oldUrlHasRootRedirect ? string.Empty : string.Concat(oldUrlPath, ","))
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

            if (rewrite.RelatedRewrites.Count > 1)
            {
                yield return !string.IsNullOrWhiteSpace(rewrite.OldUrl.Query)
                    ? string.Format(
                    "<add input=\"{{{0}:{{QUERY_STRING}}}}\" pattern=\"(.+)\" />",
                    rewrite.Id)
                    : string.Format(
                    "<add input=\"{{{0}:{{R:1}}}}\" pattern=\"(.+)\" />",
                    rewrite.Id);
            }
            else if (!string.IsNullOrEmpty(rewrite.OldUrl.Query))
            {
                yield return string.Format(
                    "<add input=\"{{QUERY_STRING}}\" pattern=\"{0}\" />",
                    rewrite.OldUrl.Query);
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