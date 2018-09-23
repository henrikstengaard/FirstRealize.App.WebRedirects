﻿using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Readers;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class ConfigurationJsonReaderTests
    {
        [Test]
        public void ConvertJsonToUri()
        {
            var uriJsonConverter = new UriJsonConverter();

            Assert.AreEqual(
                true,
                uriJsonConverter.CanConvert(typeof(Uri)));
            Assert.AreEqual(
                false,
                uriJsonConverter.CanConvert(typeof(DateTime)));

            // read uri from json
            var resultUris = new List<object>();
            var json = @"{
    'Test1': 'http://www.test.local',
    'Test2': 'not-a-valid'
}";
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                while(reader.Read())
                {
                    if (reader.TokenType == JsonToken.String &&
                        reader.Value != null)
                    {
                        resultUris.Add(uriJsonConverter.ReadJson(
                            reader,
                            typeof(Uri),
                            reader.Value,
                            new JsonSerializer()));

                    }
                }
            }

            // verify result urls
            Assert.AreEqual(2, resultUris.Count);
            var resultUri = resultUris[0] as Uri;
            Assert.IsNotNull(resultUri);
            Assert.AreEqual(
                new Uri("http://www.test.local").AbsoluteUri,
                resultUri.AbsoluteUri);
            Assert.AreEqual(
                null,
                resultUris[1]);
        }

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
    useTestHttpClient: ""True"",
    forceHttpHostPatterns: [
        ""www\\.oldurl\\.local""
    ],
    maxRedirectCount: ""50"",
    sampleCount: ""100""
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
                configuration.DefaultUrl.AbsoluteUri);
            Assert.AreEqual(
                "/oldurl-exclude",
                configuration.OldUrlExcludePatterns.FirstOrDefault());
            Assert.AreEqual(
                "/newurl-exclude",
                configuration.NewUrlExcludePatterns.FirstOrDefault());
            Assert.AreEqual(
                true,
                configuration.UseTestHttpClient);
            Assert.AreEqual(
                "www\\.oldurl\\.local",
                configuration.ForceHttpHostPatterns.FirstOrDefault());
            Assert.AreEqual(
                50,
                configuration.MaxRedirectCount);
            Assert.AreEqual(
                100,
                configuration.SampleCount);
        }
    }
}
