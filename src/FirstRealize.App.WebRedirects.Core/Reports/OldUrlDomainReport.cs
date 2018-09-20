using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Writers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class OldUrlDomainReport : IReport<OldUrlDomainRecord>
    {
        private readonly HashSet<string> _oldUrlDomainsIndex;

        public OldUrlDomainReport()
        {
            _oldUrlDomainsIndex = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        public void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            foreach(var parsedRedirect in redirectProcessingResult
                .ParsedRedirects)
            {
                if (!parsedRedirect.IsValid ||
                    _oldUrlDomainsIndex.Contains(
                        parsedRedirect.OldUrl.Parsed.Host))
                {
                    continue;
                }

                _oldUrlDomainsIndex.Add(
                    parsedRedirect.OldUrl.Parsed.Host);
            }
        }

        public IEnumerable<OldUrlDomainRecord> GetRecords()
        {
            return _oldUrlDomainsIndex
                .ToList()
                .OrderBy(x => x)
                .Select(x => new OldUrlDomainRecord
                {
                    OldUrlDomain = x
                });
        }

        public void WriteReportCsvFile(
            string path)
        {
            using (var reportCsvWriter = new ReportCsvWriter<OldUrlDomainRecord>(
                path))
            {
                reportCsvWriter.Write(this);
            }
        }
    }
}