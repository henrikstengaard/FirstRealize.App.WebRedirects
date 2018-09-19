using CsvHelper;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Reports;
using System;
using System.IO;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Writers
{
    public class ReportCsvWriter<TEntity> : IDisposable where TEntity : class
    {
        private readonly CsvWriter _csvWriter;
        private bool _disposed;

        public ReportCsvWriter(string path)
        {
            _csvWriter = new CsvWriter(
                new StreamWriter(path), new CsvHelper.Configuration.Configuration
                {
                    Delimiter = ";",
                    Quote = '"',
                    QuoteAllFields = true
                }, false);
            _csvWriter.Configuration.AutoMap<TEntity>();
            _csvWriter.WriteHeader<TEntity>();
            _csvWriter.NextRecord();
            _disposed = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _csvWriter.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Write(
            IReport<TEntity> report)
        {
            _csvWriter.WriteRecords(report.GetRecords());
        }
    }
}