using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using FirstRealize.App.WebRedirects.Core.Parsers;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public class AwsS3StaticWebsiteExporter : IExporter
    {
        private readonly IConfiguration configuration;
        private readonly IUrlParser urlParser;
        private readonly IUrlFormatter urlFormatter;
        private readonly Regex headingSlashRegex;

        private readonly string routingRulesTemplate = @"<RoutingRules>
{0}
</RoutingRules>";

        private readonly string routingRuleTemplate = @"<RoutingRule>
    <Condition>
{0}
    </Condition>
    <Redirect>
{1}
    </Redirect>
</RoutingRule>";

        public AwsS3StaticWebsiteExporter(
            IConfiguration configuration,
            IUrlParser urlParser,
            IUrlFormatter urlFormatter)
        {
            this.configuration = configuration;
            this.urlParser = urlParser;
            this.urlFormatter = urlFormatter;
            headingSlashRegex = new Regex(
                "^[/]+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        }

        public string Name => "AwsS3StaticWebsite";

        public string Build(IEnumerable<IRedirect> redirects)
        {
            var routingRules = new List<string>();

            foreach (var redirect in redirects)
            {
                var oldUrlParsed = urlParser.Parse(
                    redirect.OldUrl,
                    configuration.DefaultUrl);
                var newUrlParsed = urlParser.Parse(
                    redirect.NewUrl,
                    configuration.DefaultUrl);

                var oldUrlHasRootPath = oldUrlParsed.Path.Equals("/");

                if (configuration.ExcludeOldUrlRootRedirects && oldUrlHasRootPath)
                {
                    continue;
                }

                routingRules.Add(
                    string.Format(
                        routingRuleTemplate,
                        BuildCondition(oldUrlParsed),
                        BuildRedirect(newUrlParsed)));
            }

            return string.Format(
                routingRulesTemplate,
                string.Join(Environment.NewLine, routingRules));
        }

        public void Export(IEnumerable<IRedirect> redirects, string outputDir)
        {
            var awsS3StaticWebsiteXmlFile = Path.Combine(
                outputDir,
                "awsS3StaticWebsite.xml");

            File.WriteAllText(
                awsS3StaticWebsiteXmlFile,
                Build(redirects));
        }

        public string BuildCondition(IParsedUrl oldUrlParsed)
        {
            return $"<KeyPrefixEquals>{FormatPathAndQuery(oldUrlParsed.Path)}</KeyPrefixEquals>{Environment.NewLine}";
        }

        public string BuildRedirect(IParsedUrl newUrlParsed)
        {
            var redirectLines = new List<string>(
                new []
                {
                    $"<ReplaceKeyPrefixWith>{XmlEncode(FormatPathAndQuery(newUrlParsed.PathAndQuery))}</ReplaceKeyPrefixWith>",
                    "<HttpRedirectCode>301</HttpRedirectCode>"
                }
            );

            if (newUrlParsed.OriginalUrlHasHost)
            {
                redirectLines.AddRange(
                    new []
                    {
                        $"<Protocol>{newUrlParsed.Scheme}</Protocol>",
                        $"<HostName>{newUrlParsed.Host}</HostName>",
                    }
                );
            }

            return string.Join(
                Environment.NewLine, redirectLines);
        }

        private string FormatPathAndQuery(
            string pathAndQuery)
        {
            return headingSlashRegex.Replace(
                pathAndQuery,
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