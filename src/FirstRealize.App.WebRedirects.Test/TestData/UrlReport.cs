using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Reports;

namespace FirstRealize.App.WebRedirects.Test.TestData
{
    class UrlReport : ReportBase<UrlReportRecord>
    {
        private readonly IEnumerable<UrlReportRecord> _urlReportRecords;

        public UrlReport(
            IEnumerable<UrlReportRecord> urlReportRecords)
        {
            _urlReportRecords = urlReportRecords;
        }

        public override void Build(IRedirectProcessingResult redirectEngine)
        {
        }

        public override IEnumerable<UrlReportRecord> GetRecords()
        {
            return _urlReportRecords;
        }
    }
}