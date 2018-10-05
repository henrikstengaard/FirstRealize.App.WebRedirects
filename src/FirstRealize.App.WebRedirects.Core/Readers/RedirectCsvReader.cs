﻿using CsvHelper;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;
using System.Collections.Generic;
using System.IO;

namespace FirstRealize.App.WebRedirects.Core.Readers
{
    public class RedirectCsvReader : IDisposable
    {
        private readonly CsvReader _csvReader;
        private bool _disposed;

        public RedirectCsvReader(string path)
        {
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
                yield return new Redirect
                {
                    OldUrl = _csvReader.GetField<string>("oldurl"),
                    NewUrl = _csvReader.GetField<string>("newurl"),
                    OldUrlHasHost = _csvReader.GetField<bool>("oldurlhashost"),
                    NewUrlHasHost = _csvReader.GetField<bool>("newurlhashost"),
                    ParsedOldUrl = _csvReader.GetField<string>("parsedoldurl"),
                    ParsedNewUrl = _csvReader.GetField<string>("parsednewurl"),
                    OriginalOldUrl = _csvReader.GetField<string>("originaloldurl"),
                    OriginalNewUrl = _csvReader.GetField<string>("originalnewurl"),
                    OriginalOldUrlHasHost = _csvReader.GetField<bool>("oldurlhashost"),
                    OriginalNewUrlHasHost = _csvReader.GetField<bool>("newurlhashost"),
                };
            }
        }
    }
}