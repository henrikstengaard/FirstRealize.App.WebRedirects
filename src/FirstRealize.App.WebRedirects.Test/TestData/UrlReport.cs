using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Reports;

namespace FirstRealize.App.WebRedirects.Test.TestData
{
    class UrlReport : IReport<UrlReportRecord>
    {
        private readonly IEnumerable<UrlReportRecord> _urlReportRecords;

        public UrlReport(
            IEnumerable<UrlReportRecord> urlReportRecords)
        {
            _urlReportRecords = urlReportRecords;
        }

        public void Build(IRedirectProcessingResult redirectEngine)
        {
        }

        public IEnumerable<UrlReportRecord> GetRecords()
        {
            return _urlReportRecords;
        }
    }
}