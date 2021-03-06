﻿using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Reports;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReportTests
{
    [TestFixture]
    public class OldUrlDomainReportTests
    {
        [Test]
        public void BuildOldUrlDomainReport()
        {
            var redirectProcessingResult = new RedirectProcessingResult
            {
                ParsedRedirects = TestData.TestData.GetParsedRedirects(
                new[]
                {
                    new Redirect
                    {
                        OldUrl = "http://www.test1.local/url1",
                        NewUrl = "http://www.test3.local"
                    },
                    new Redirect
                    {
                        OldUrl = "http://www.test1.local/url2",
                        NewUrl = "http://www.test3.local"
                    },
                    new Redirect
                    {
                        OldUrl = "http://www.test2.local/url3",
                        NewUrl = "http://www.test3.local"
                    }
                })
            };

            // create and build old url domain report
            var oldUrlDomainReport = new OldUrlDomainReport();
            oldUrlDomainReport.Build(redirectProcessingResult);

            // verify old url domains records are build
            var records = oldUrlDomainReport
                .GetRecords()
                .OfType<OldUrlDomainRecord>()
                .ToList();
            Assert.AreEqual(2, records.Count);
            Assert.AreEqual(
                "www.test1.local",
                records[0].OldUrlDomain);
            Assert.AreEqual(
                "www.test2.local",
                records[1].OldUrlDomain);
        }
    }
}