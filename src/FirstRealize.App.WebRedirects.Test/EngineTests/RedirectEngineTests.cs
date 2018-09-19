using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Readers;
using FirstRealize.App.WebRedirects.Test.Clients;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.EngineTests
{
    [TestFixture]
    public class RedirectEngineTests
    {
        [Test]
        public void ProcessedRedirectsWithResults()
        {
            // read configuration file
            IConfiguration configuration;
            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                configuration = configurationJsonReader
                    .ReadConfiguationFile(
                    Path.Combine(TestData.TestData.CurrentDirectory, @"TestData\configuration.json"));
            }

            // create redirect engine
            var urlParser = new UrlParser();
            var redirectEngine = new RedirectEngine(
                configuration,
                urlParser,
                new RedirectParser(
                    configuration,
                    urlParser),
                new ControlledHttpClient());

            // run redirect engine
            redirectEngine.Run();

            // verify redirect engine has processed redirects with results
            var processedRedirects = redirectEngine
                .ProcessedRedirects
                .ToList();
            Assert.AreNotEqual(0, processedRedirects.Count);
            Assert.AreNotEqual(0, processedRedirects
                .Count(pr => pr.Results
                .Any(r => r.Type.Equals(ResultTypes.DuplicateOfFirst))));
            Assert.AreNotEqual(0, processedRedirects
                .Count(pr => pr.Results
                .Any(r => r.Type.Equals(ResultTypes.DuplicateOfLast))));
            Assert.AreNotEqual(0, processedRedirects
                .Count(pr => pr.Results
                .Any(r => r.Type.Equals(ResultTypes.CyclicRedirect))));

            // verify redirect engine has results
            var results = redirectEngine
                .Results
                .ToList();
            Assert.AreNotEqual(0, results
                .Count(r => r.Type.Equals(ResultTypes.DuplicateOfFirst)));
            Assert.AreNotEqual(0, results
                .Count(r => r.Type.Equals(ResultTypes.DuplicateOfLast)));
            Assert.AreNotEqual(0, results
                .Count(r => r.Type.Equals(ResultTypes.CyclicRedirect)));
        }
    }
}