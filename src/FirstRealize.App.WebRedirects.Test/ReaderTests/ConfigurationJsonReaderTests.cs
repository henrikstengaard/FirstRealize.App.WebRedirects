﻿using FirstRealize.App.WebRedirects.Core.Configuration;
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
    defaultOldUrl: ""http://www.oldurl.local"",
    defaultNewUrl: ""http://www.newurl.local"",
    oldUrlExcludePatterns: [
        ""/oldurl-exclude""
    ],
    newUrlExcludePatterns: [
        ""/newurl-exclude""
    ],
    forceHttp: ""True""
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
                configuration.DefaultOldUrl.AbsoluteUri);
            Assert.AreEqual(
                "http://www.newurl.local/",
                configuration.DefaultNewUrl.AbsoluteUri);
            Assert.AreEqual(
                "/oldurl-exclude",
                configuration.OldUrlExcludePatterns.FirstOrDefault());
            Assert.AreEqual(
                "/newurl-exclude",
                configuration.NewUrlExcludePatterns.FirstOrDefault());
            Assert.AreEqual(
                true,
                configuration.ForceHttp);
        }
    }
}
