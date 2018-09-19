using FirstRealize.App.WebRedirects.Core.Writers;
using FirstRealize.App.WebRedirects.Test.TestData;
using NUnit.Framework;
using System.IO;

namespace FirstRealize.App.WebRedirects.Test.WriterTests
{
    [TestFixture]
    public class ReportCsvWriterTests
    {
        [Test]
        public void WriteReportCsvFile()
        {
            // create url report
            var urlReport = new UrlReport(
                new[]
                {
                    new UrlReportRecord
                    {
                        Url = "/url"
                    }
                });

            // write url report
            var reportCsvFile = Path.Combine(
                TestData.TestData.CurrentDirectory,
                @"report.csv");
            using (var reportCsvWriter = new ReportCsvWriter<UrlReportRecord>(reportCsvFile))
            {
                reportCsvWriter.Write(urlReport);
            }

            // read url report lines
            var urlReportLines =
                File.ReadAllLines(reportCsvFile);

            // verify url report lines
            Assert.AreEqual(2, urlReportLines.Length);
            Assert.AreEqual(
                "\"Url\"",
                urlReportLines[0]);
            Assert.AreEqual(
                "\"/url\"",
                urlReportLines[1]);
        }
    }
}