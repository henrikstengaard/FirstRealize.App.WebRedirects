using CsvHelper;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;
using System.Collections.Generic;
using System.IO;

namespace FirstRealize.App.WebRedirects.Core.Readers
{
    public class RedirectCsvReader : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly CsvReader _csvReader;
        private bool _disposed;

        public RedirectCsvReader(
            IConfiguration configuration,
            string path)
        {
            _configuration = configuration;
            _csvReader = new CsvReader(
                new StreamReader(path), new CsvHelper.Configuration.Configuration
                {
                    Delimiter = ";",
                    PrepareHeaderForMatch = header => header.ToLower(),
                    MissingFieldFound = null
                }, false);
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
                _csvReader.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IRedirect> ReadAllRedirects()
        {
            if (_csvReader.Read())
            {
                _csvReader.ReadHeader();
            }

            while (_csvReader.Read())
            {
                var redirectType = _csvReader.GetField<string>("redirecttype");

                RedirectType parsedRedirectType = _configuration.DefaultRedirectType;
                if (!string.IsNullOrWhiteSpace(redirectType) &&
                    !Enum.TryParse(redirectType, out parsedRedirectType))
                {
                    parsedRedirectType = _configuration.DefaultRedirectType;
                }

                yield return new Redirect
                {
                    OldUrl = TrimWhitespace(
                        _csvReader.GetField<string>("oldurl")),
                    NewUrl = TrimWhitespace(
                        _csvReader.GetField<string>("newurl")),
                    OldUrlHasHost =
                        _csvReader.GetField<bool>("oldurlhashost"),
                    NewUrlHasHost =
                        _csvReader.GetField<bool>("newurlhashost"),
                    ParsedOldUrl = TrimWhitespace(
                        _csvReader.GetField<string>("parsedoldurl")),
                    ParsedNewUrl = TrimWhitespace(
                        _csvReader.GetField<string>("parsednewurl")),
                    OriginalOldUrl = TrimWhitespace(
                        _csvReader.GetField<string>("originaloldurl")),
                    OriginalNewUrl = TrimWhitespace(
                        _csvReader.GetField<string>("originalnewurl")),
                    OriginalOldUrlHasHost =
                        _csvReader.GetField<bool>("oldurlhashost"),
                    OriginalNewUrlHasHost =
                        _csvReader.GetField<bool>("newurlhashost"),
                    RedirectType = parsedRedirectType
                };
            }
        }

        private string TrimWhitespace(string value)
        {
            if (value == null)
            {
                return value;
            }

            return value.Trim();
        }
    }
}