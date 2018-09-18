using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Readers;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class ConfigurationJsonReaderTests
    {
        [Test]
        public void CanReadConfigurationJson()
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
            IConfiguration configuration;
            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                configuration = configurationJsonReader
                    .ReadConfiguationJson(configurationJson);
            }

            Assert.IsNotNull(configuration);
            var redirectCsvFiles = configuration.RedirectCsvFiles.ToList();
            Assert.AreEqual(2, redirectCsvFiles.Count);
            Assert.AreEqual(
                "redirects1.csv",
                redirectCsvFiles[0]);
            Assert.AreEqual(
                "redirects2.csv",
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
