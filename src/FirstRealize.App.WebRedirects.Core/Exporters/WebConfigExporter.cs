using FirstRealize.App.WebRedirects.Core.Extensions;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public class WebConfigExporter : IExporter
    {
        private readonly IUrlParser _urlParser;
        private readonly IUrlFormatter _urlFormatter;

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
</configuration>
";

        private readonly string _rewriteRuleTemplate = @"<rule name=""{0}"" stopProcessing=""true"">
    <match url=""{1}"" />
    <conditions>
{2}
    </conditions>
    <action type=""Redirect"" url=""{3}"" redirectType=""Permanent"" appendQueryString=""False"" />
</rule>";

        private readonly string _rewriteMapTemplate = @"<rewriteMap name=""{0}"" defaultValue="""">
{1}
</rewriteMap>";

        public WebConfigExporter(
            IUrlParser urlParser,
            IUrlFormatter urlFormatter)
        {
            _urlParser = urlParser;
            _urlFormatter = urlFormatter;
        }

        public string Name => "WebConfig";

        public string Build(
            IEnumerable<IRedirect> redirects)
        {
            var rewritesIndex = new Dictionary<string, Rewrite>();

            foreach (var redirect in redirects)
            {
                var oldUrlRefined = _urlParser.Parse(
                    redirect.OldUrlRefined);
                var newUrlRefined = _urlParser.Parse(
                    redirect.NewUrlRefined);

                //var oldHost = if ($forceOldUrlDomainHost - and $oldUrlDomainUri) { $oldUrlDomainUri.Host } else { $redirect.OldHost }

                var redirectUrl = redirect.NewUrlHasHost
                    ? string.Format("{0}://{1}{{C:1}}", newUrlRefined.Scheme, newUrlRefined.Host)
                    : "{C:1}";

                // build name
                var name = new StringBuilder("Rewrite rule for ");

                var isRootRedirect = oldUrlRefined.Path.Equals("/");

                if (isRootRedirect)
                {
                    name.Append("root url ");
                }
                else
                {
                    name.Append("urls ");
                }

                var key = new StringBuilder(
                    string.Format("OldUrlHost={0}", redirect.OldUrlHasHost));
                if (redirect.OldUrlHasHost)
                {
                    name.Append(
                        string.Format("from host '{0}'", oldUrlRefined.Host));
                    key.Append(
                        string.Format(",{0}", oldUrlRefined.Host.ToLower()));
                }
                else
                {
                    name.Append("from any host");
                }
                key.Append(
                    string.Format("|NewUrlHost={0}", redirect.NewUrlHasHost));
        
                if (redirect.NewUrlHasHost)
                {
                    name.Append(
                        string.Format(" to host '{0}'", newUrlRefined.Host));
                    key.Append(
                        string.Format(",{0}", newUrlRefined.Host.ToLower()));
                }
                else
                {
                    name.Append(" to same host");
                }

                // make root redirects unique
                if (isRootRedirect)
                {
                    key.Append("|/");
                }

                var useRewriteMap = false;
                var useQueryString = false;

                string matchUrl;
                if (isRootRedirect)
                {
                    matchUrl = "^/?$";
                }
                else if(string.IsNullOrWhiteSpace(oldUrlRefined.Query))
                {
                    useRewriteMap = true;
                    matchUrl = "^(.+?)/?$";
                }
                else
                {
                    name.Append(
                        string.Format(" using query string '{0}'", oldUrlRefined.Query));
                    key.Append(
                        string.Format("|{0}", oldUrlRefined.Query));

                    useRewriteMap = true;
                    useQueryString = true;
                    matchUrl = "^(.+?)/?$";
                }

                var conditions = new List<string>();

                var id = key.ToString().ToMd5().ToLower();

                if (redirect.OldUrlHasHost && isRootRedirect)
                {
                    conditions.Add(
                        string.Format("<add input=\"{{HTTP_HOST}}\" pattern=\"^{0}$\" />", oldUrlRefined.Host));
                }

                if (useQueryString)
                {
                    conditions.Add(
                        string.Format(
                            "<add input=\"{{QUERY_STRING}}\" pattern=\"{0}\" />",
                            XmlEncode(Regex.Escape(oldUrlRefined.Query))));
                }

                if (useRewriteMap)
                {
                    conditions.Add(
                        string.Format("<add input=\"{{{0}:{{R:1}}}}\" pattern=\"(.+)\" />", id));
                }

                if (!rewritesIndex.ContainsKey(id))
                {
                    rewritesIndex.Add(id, new Rewrite
                    {
                        Id = id,
                        Name = name.ToString(),
                        Redirect = redirect,
                        OldUrl = oldUrlRefined,
                        NewUrl = newUrlRefined,
                        MatchUrl = matchUrl,
                        RedirectUrl = redirectUrl
                    });
                }

                // add rewrite to rewrite rules index
                var oldPath = FormatOldPath(oldUrlRefined.Path);
                var newPath = FormatNewPathAndQuery(newUrlRefined.Path, newUrlRefined.Query);

                rewritesIndex[id].RewriteMap[oldPath] = newPath;
            }

            // optimize
            foreach (var rewrite in rewritesIndex.Keys
                .Where(id => rewritesIndex[id].RewriteMap.Keys.Count == 1)
                .Select(id => rewritesIndex[id]))
            {
                // replace match url with first old path in rewrite map
                rewrite.MatchUrl = string.Format(
                    "^{0}/?$", FormatOldPath(rewrite.OldUrl.Path));

                // remove rewrite maps
                rewrite.RewriteMap.Clear();

                // replace redirect url with new url
                rewrite.RedirectUrl = XmlEncode(rewrite.Redirect.NewUrlRefined);
            }

            // build rewrite maps
            var rewriteMaps = new List<string>();
            foreach(var id in rewritesIndex.Keys
                .Where(id => rewritesIndex[id].RewriteMap.Keys.Count > 1)
                .OrderBy(x => x))
            {
                rewriteMaps.Add(
                    string.Format(
                        _rewriteMapTemplate,
                        id, 
                        string.Join(Environment.NewLine,rewritesIndex[id].RewriteMap.Keys.OrderByDescending(x => x).Select(x => string.Format(
                            "<add key=\"{0}\" value=\"{1}\" />",
                            XmlEncode(x),
                            XmlEncode(rewritesIndex[id].RewriteMap[x])
                            )))));
            }

            // sort rewrite index for building rewrite rules
            var rewriteIdsSorted = rewritesIndex.Keys
                .Where(x => !string.IsNullOrWhiteSpace(rewritesIndex[x].OldUrl.Query))
                .OrderByDescending(x => rewritesIndex[x].OldUrl.PathAndQuery)
                .OrderByDescending(x => rewritesIndex[x].Redirect.OldUrlHasHost)
                .OrderByDescending(x => rewritesIndex[x].Redirect.NewUrlHasHost)
                .Concat(rewritesIndex.Keys
                .Where(x => string.IsNullOrWhiteSpace(rewritesIndex[x].OldUrl.Query))
                .OrderByDescending(x => rewritesIndex[x].OldUrl.PathAndQuery)
                .OrderByDescending(x => rewritesIndex[x].Redirect.OldUrlHasHost)
                .OrderByDescending(x => rewritesIndex[x].Redirect.NewUrlHasHost));

            // build rewrite rules
            var rewriteRules = new List<string>();

            foreach (var rewrite in rewriteIdsSorted.Select(x => rewritesIndex[x]))
            {
                rewriteRules.Add(string.Format(
                    _rewriteRuleTemplate,
                    rewrite.Name,
                    rewrite.MatchUrl,
                    string.Join(Environment.NewLine, BuildConditions(rewrite)),
                    rewrite.RedirectUrl));
            }

            return string.Format(
                _webConfigTemplate,
                string.Join(Environment.NewLine, rewriteRules),
                string.Join(Environment.NewLine, rewriteMaps));
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
            if (rewrite.Redirect.OldUrlHasHost &&
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

            if (rewrite.RewriteMap.Count > 0)
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
            return Regex.Replace(
                path,
                "^/",
                string.Empty,
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
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