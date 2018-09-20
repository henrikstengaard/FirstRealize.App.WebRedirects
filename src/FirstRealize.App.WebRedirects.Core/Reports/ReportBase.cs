using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Writers;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public abstract class ReportBase<T> : IReport<T> where T : class
    {
        public abstract void Build(
            IRedirectProcessingResult redirectEngine);

        public abstract IEnumerable<T> GetRecords();

        public virtual void WriteReportCsvFile(
            string path)
        {
            using (var reportCsvWriter = new ReportCsvWriter<T>(
                path))
            {
                reportCsvWriter.Write(this);
            }
        }
    }
}