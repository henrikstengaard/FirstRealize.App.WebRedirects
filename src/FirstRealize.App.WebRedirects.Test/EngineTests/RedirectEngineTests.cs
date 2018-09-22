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
        private Configuration CreateConfiguration()
        {
            // read configuration file
            IConfiguration configuration;
            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                configuration = configurationJsonReader
                    .ReadConfiguationFile(
                    Path.Combine(TestData.TestData.CurrentDirectory, @"TestData\configuration.json"));
            }

            return configuration as Configuration;
        }

        private IRedirectEngine CreateRedirectEngine(
            IConfiguration configuration)
        {
            // create redirect engine
            var urlParser = new UrlParser();
            return new RedirectEngine(
                configuration,
                urlParser,
                new RedirectParser(
                    configuration,
                    urlParser),
                new TestHttpClient());
        }

        [Test]
        public void RunDefaultProcessors()
        {
            // create redirect engine
            var redirectEngine = CreateRedirectEngine(
                CreateConfiguration());

            // run redirect engine
            var redirectProcessingResult = 
                redirectEngine.Run();

            // verify redirect engine has processed redirects with results
            var processedRedirects = redirectProcessingResult
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
            var results = redirectProcessingResult
                .Results
                .ToList();
            Assert.AreNotEqual(0, results
                .Count(r => r.Type.Equals(ResultTypes.DuplicateOfFirst)));
            Assert.AreNotEqual(0, results
                .Count(r => r.Type.Equals(ResultTypes.DuplicateOfLast)));
            Assert.AreNotEqual(0, results
                .Count(r => r.Type.Equals(ResultTypes.CyclicRedirect)));
        }

        [Test]
        public void ChangeActiveProcessors()
        {
            // create and customize configuration
            var testProcessor = new TestProcessor();
            var configuration = CreateConfiguration();
            var customizedConfiguration = new Configuration
            {
                RedirectCsvFiles = configuration.RedirectCsvFiles,
                DefaultOldUrl = configuration.DefaultOldUrl,
                DefaultNewUrl = configuration.DefaultNewUrl,
                Processors = new[]
                {
                    testProcessor.Name
                }
            };

            // create redirect engine
            var redirectEngine = CreateRedirectEngine(
                customizedConfiguration);
            redirectEngine.Processors.Add(
                testProcessor);

            // run redirect engine
            var redirectProcessingResult =
                redirectEngine.Run();

            // verify redirect engine processed redirects only has test processor results
            var processedRedirects = redirectProcessingResult
                .ProcessedRedirects
                .ToList();
            Assert.AreNotEqual(
                0, processedRedirects.Count);
            Assert.AreEqual(
                processedRedirects.Count,
                processedRedirects.Count(
                    pr => pr.Results.Count() == 1 && pr.Results.All(
                        r => r.Type.Equals(testProcessor.Name))));
        }

        [Test]
        public void SampleProcessedRedirects()
        {
            var configuration = CreateConfiguration();
            configuration.SampleCount = 1;

            // create redirect engine
            var redirectEngine = CreateRedirectEngine(
                configuration);

            // run redirect engine
            var redirectProcessingResult =
                redirectEngine.Run();

            // verify redirect engine has processed redirects with results
            var processedRedirects = redirectProcessingResult
                .ProcessedRedirects
                .ToList();

            // verify parsed more than 1 redirect and processed only 1 redirect controlled by sample count set to 1
            Assert.IsTrue(
                redirectProcessingResult
                .ParsedRedirects
                .Count() > 1);
            Assert.AreEqual(
                1,
                redirectProcessingResult
                .ProcessedRedirects
                .Count()
                );
        }

        [Test]
        public void DetectUnkownErrors()
        {
            // create and customize configuration
            var causeErrorProcessor = new CauseErrorProcessor();
            var configuration = CreateConfiguration();
            var customizedConfiguration = new Configuration
            {
                RedirectCsvFiles = configuration.RedirectCsvFiles,
                DefaultOldUrl = configuration.DefaultOldUrl,
                DefaultNewUrl = configuration.DefaultNewUrl,
                Processors = new[]
                {
                    causeErrorProcessor.Name
                }
            };

            // create redirect engine
            var redirectEngine = CreateRedirectEngine(
                customizedConfiguration);
            redirectEngine.Processors.Add(
                causeErrorProcessor);

            // run redirect engine
            var redirectProcessingResult =
                redirectEngine.Run();

            // verify redirect engine processed redirects and has unknown error result
            var processedRedirects = redirectProcessingResult
                .ProcessedRedirects
                .ToList();
            Assert.AreNotEqual(
                0, processedRedirects.Count);
            Assert.AreEqual(
                processedRedirects.Count,
                processedRedirects.Count(
                    pr => pr.Results.Count() == 1 && pr.Results.All(
                        r => r.Type.Equals(ResultTypes.UnknownErrorResult))));
        }
    }
}