using FirstRealize.App.WebRedirects.Core.Exporters;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Test.ExportTests
{
    [TestFixture]
    public class AwsS3StaticWebsiteExporterTests
    {
        [Test]
        public void BuildAwsS3StaticWebsiteWithoutHost()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var awsS3StaticWebsiteExporter = new AwsS3StaticWebsiteExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "http://www.test.local/url1",
                    NewUrl = "http://www.test.local/url2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/url1",
                    ParsedNewUrl = "http://www.test.local/url2",
                    OriginalOldUrl = "/url1",
                    OriginalNewUrl = "/url2",
                }
            };

            var awsS3StaticWebsite = awsS3StaticWebsiteExporter.Build(
                redirects);

            // verify aws s3 static website
            Assert.IsNotNull(awsS3StaticWebsite);
            var awsS3StaticWebsiteLines = awsS3StaticWebsite
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                awsS3StaticWebsiteLines.Count);
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains($"<KeyPrefixEquals>url1</KeyPrefixEquals>")));
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains("<ReplaceKeyPrefixWith>url2</ReplaceKeyPrefixWith>")));
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains("<HttpRedirectCode>301</HttpRedirectCode>")));
            Assert.AreEqual(
                0,
                awsS3StaticWebsiteLines.Count(
                    x => Regex.IsMatch(x, "^\\s*<(Protocol|HostName|HttpRedirectCode)>\\s*$", RegexOptions.IgnoreCase)));
        }

        [Test]
        public void BuildAwsS3StaticWebsiteWithHost()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var awsS3StaticWebsiteExporter = new AwsS3StaticWebsiteExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "/url1",
                    NewUrl = "http://www.other.local/url2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = true,
                    ParsedOldUrl = "http://www.test.local/url1",
                    ParsedNewUrl = "http://www.other.local/url2",
                    OriginalOldUrl = "/url1",
                    OriginalNewUrl = "http://www.other.local/url2",
                }
            };

            var awsS3StaticWebsite = awsS3StaticWebsiteExporter.Build(
                redirects);

            // verify aws s3 static website
            Assert.IsNotNull(awsS3StaticWebsite);
            var awsS3StaticWebsiteLines = awsS3StaticWebsite
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                awsS3StaticWebsiteLines.Count);
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains($"<KeyPrefixEquals>url1</KeyPrefixEquals>")));
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains("<ReplaceKeyPrefixWith>url2</ReplaceKeyPrefixWith>")));
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains("<Protocol>http</Protocol>")));
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains("<HostName>www.other.local</HostName>")));
            Assert.AreEqual(
                1,
                awsS3StaticWebsiteLines.Count(
                    x => x.Contains("<HttpRedirectCode>301</HttpRedirectCode>")));
        }
    }
}