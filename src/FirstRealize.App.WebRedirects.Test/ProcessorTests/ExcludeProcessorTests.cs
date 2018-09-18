﻿using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class ExcludeProcessorTests
    {
        [Test]
        public void CanProcessExcludedRedirects()
        {
            var processedRedirects = TestData.GetProcessedRedirects(
                new[]
                {
                    new ExcludeProcessor(new Configuration
                    {
                        OldUrlExcludePatterns = new []{ "new-url" },
                        NewUrlExcludePatterns = new []{ "/redirect/somwhere/else" }
                    })
                });

            var excludedRedirectsMatchingOldUrl = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Excluded)) &&
                pr.Redirect.OldUrl.Parsed.AbsoluteUri.Contains("new-url"))
                .ToList();
            Assert.AreEqual(2, excludedRedirectsMatchingOldUrl.Count);
            foreach(var excludedRedirect in excludedRedirectsMatchingOldUrl)
            {
                Assert.AreEqual(
                    "http://www.test.local/new-url",
                    excludedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri);
            }

            var excludedRedirectsMatchingNewUrl = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Excluded)) &&
                pr.Redirect.NewUrl.Parsed.AbsoluteUri.Contains("/redirect/somwhere/else"))
                .ToList();
            var excludedRedirectMatchingNewUrl = excludedRedirectsMatchingNewUrl
                .FirstOrDefault();
            Assert.AreEqual(1, excludedRedirectsMatchingNewUrl.Count);
            Assert.IsNotNull(excludedRedirectMatchingNewUrl);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                excludedRedirectMatchingNewUrl.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}