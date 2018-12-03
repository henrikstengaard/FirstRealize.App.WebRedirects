using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
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
            // write empty redirects csv files
            var redirects1CsvFile = Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "redirects1.csv");
            File.WriteAllText(
                redirects1CsvFile,
                string.Empty);
            var redirects2CsvFile = Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "redirects2.csv");
            File.WriteAllText(
                redirects2CsvFile,
                string.Empty);

            // configuration json
            var configurationJson = @"{{
    redirectCsvFiles: [
        ""{0}"",
        ""{1}""
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
    export: ""True"",
    httpClientTimeout: 600,
    defaultRedirectType: ""Replace""
}}";

            // write configuration file
            var configurationFile =
                Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "test_configuration.json");
            File.WriteAllText(
                configurationFile,
                string.Format(
                    configurationJson,
                    redirects1CsvFile.Replace("\\", "\\\\"),
                    redirects2CsvFile.Replace("\\", "\\\\")));

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
                redirects1CsvFile,
                redirectCsvFiles[0]);
            Assert.AreEqual(
                redirects2CsvFile,
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
            Assert.AreEqual(
                600,
                configuration.HttpClientTimeout);
            Assert.AreEqual(
                RedirectType.Replace,
                configuration.DefaultRedirectType);
        }

        [Test]
        public void CurrentDirUsedForRedirectCsvFiles()
        {
            // write empty redirects csv files
            var redirectsCsvFile = Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "redirects.csv");
            File.WriteAllText(
                redirectsCsvFile,
                string.Empty);

            // configuration json
            var configurationJson = @"{
    redirectCsvFiles: [
        ""redirects.csv""
    ],
}";
            // write configuration file
            var configurationFile =
                Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "test_configuration.json");
            File.WriteAllText(
                configurationFile,
                configurationJson);

            IConfiguration configuration;
            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                configuration = configurationJsonReader
                    .ReadConfiguationFile(configurationFile);
            }

            Assert.IsNotNull(configuration);
            var redirectCsvFiles = configuration.RedirectCsvFiles.ToList();
            Assert.AreEqual(1, redirectCsvFiles.Count);
            Assert.AreEqual(
                redirectsCsvFile,
                redirectCsvFiles[0]);
        }

        [Test]
        public void NonExistingRedirectCsvFilesThrowsException()
        {
            var configurationJson = @"{
    redirectCsvFiles: [
        ""non-existing-redirects.csv""
    ],
}";
            // write configuration file
            var configurationFile =
                Path.Combine(
                    TestData.TestData.CurrentDirectory,
                    "test_configuration.json");
            File.WriteAllText(
                configurationFile,
                configurationJson);

            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                Assert.Throws<FileNotFoundException>(() => configurationJsonReader
                    .ReadConfiguationFile(configurationFile));
            }
        }
    }
}
