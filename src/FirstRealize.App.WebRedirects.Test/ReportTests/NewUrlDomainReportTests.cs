using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Reports;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReportTests
{
    [TestFixture]
    public class NewUrlDomainReportTests
    {
        [Test]
        public void BuildNewUrlDomainReport()
        {
            var redirectProcessingResult = new RedirectProcessingResult
            {
                ParsedRedirects = TestData.TestData.GetParsedRedirects(
                new[]
                {
                    new Redirect
                    {
                        OldUrl = "http://www.test1.local/url1",
                        NewUrl = "http://www.test3.local/url8"
                    },
                    new Redirect
                    {
                        OldUrl = "http://www.test1.local/url2",
                        NewUrl = "http://www.test2.local/url5"
                    },
                    new Redirect
                    {
                        OldUrl = "http://www.test2.local/url3",
                        NewUrl = "http://www.test2.local/url9"
                    }
                })
            };

            // create and build new url domain report
            var newUrlDomainReport = new NewUrlDomainReport();
            newUrlDomainReport.Build(redirectProcessingResult);

            // verify new url domains records are build
            var records = newUrlDomainReport
                .GetRecords()
                .ToList();
            Assert.AreEqual(2, records.Count);
            Assert.AreEqual(
                "www.test2.local",
                records[0].NewUrlDomain);
            Assert.AreEqual(
                "www.test3.local",
                records[1].NewUrlDomain);
        }
    }
}
