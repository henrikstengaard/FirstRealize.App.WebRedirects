using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Readers;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class ConfigurationJsonReaderTests
    {
        [Test]
        public void ReadConfigurationFile()
        {
            var configurationJson = @"{
    redirectCsvFiles: [
        ""redirects1.csv"",
        ""redirects2.csv""
    ],
    defaultUrl: ""http://www.oldurl.local"",
    oldUrlExcludePatterns: [
        ""/oldurl-exclude""
    ],
    newUrlExcludePatterns: [
        ""/newurl-exclude""
    ],
    duplicateOldUrlStrategy: ""KeepLast"",
    excludeOldUrlRootRedirects: ""True"",
    useTestHttpClient: ""True"",
    testHttpClientNewUrlStatusCode: ""200"",
    forceHttpHostPatterns: [
        ""www\\.oldurl\\.local""
    ],
    maxRedirectCount: ""50"",
    sampleCount: ""100"",
    export: ""True""
}";
            // write configuration file
            var configurationFile =
                Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "test_configuration.json");
            File.WriteAllText(
                configurationFile,
                configurationJson);

            // write empty redirects csv files
            File.WriteAllText(
                Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "redirects1.csv"),
                string.Empty);
            File.WriteAllText(
                Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "redirects2.csv"),
                string.Empty);

            IConfiguration configuration;
            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                configuration = configurationJsonReader
                    .ReadConfiguationFile(configurationFile);
            }

            var urlFormatter = new UrlFormatter();

            Assert.IsNotNull(configuration);
            var redirectCsvFiles = configuration.RedirectCsvFiles.ToList();
            Assert.AreEqual(2, redirectCsvFiles.Count);
            Assert.AreEqual(
                Path.Combine(
                    TestData.TestData.CurrentDirectory, "redirects1.csv"),
                redirectCsvFiles[0]);
            Assert.AreEqual(
                Path.Combine(
                    TestData.TestData.CurrentDirectory, "redirects2.csv"),
                redirectCsvFiles[1]);
            Assert.AreEqual(
                "http://www.oldurl.local/",
                urlFormatter.Format(configuration.DefaultUrl));
            Assert.AreEqual(
                "/oldurl-exclude",
                configuration.OldUrlExcludePatterns.FirstOrDefault());
            Assert.AreEqual(
                "/newurl-exclude",
                configuration.NewUrlExcludePatterns.FirstOrDefault());
            Assert.AreEqual(
                DuplicateUrlStrategy.KeepLast,
                configuration.DuplicateOldUrlStrategy);
            Assert.AreEqual(
                true,
                configuration.ExcludeOldUrlRootRedirects);
            Assert.AreEqual(
                true,
                configuration.UseTestHttpClient);
            Assert.AreEqual(
                200,
                configuration.TestHttpClientNewUrlStatusCode);
            Assert.AreEqual(
                "www\\.oldurl\\.local",
                configuration.ForceHttpHostPatterns.FirstOrDefault());
            Assert.AreEqual(
                50,
                configuration.MaxRedirectCount);
            Assert.AreEqual(
                100,
                configuration.SampleCount);
            Assert.AreEqual(
                true,
                configuration.Export);
        }
    }
}
