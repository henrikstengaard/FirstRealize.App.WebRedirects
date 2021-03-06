﻿using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Exporters;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using FirstRealize.App.WebRedirects.Core.Readers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectEngine : IRedirectEngine
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlHelper _urlHelper;
        private readonly IUrlParser _urlParser;
        private readonly IRedirectParser _redirectParser;
        private readonly IHttpClient _httpClient;
        private readonly List<IRedirect> _redirects;
        private readonly List<IParsedRedirect> _parsedRedirects;
        private readonly List<IProcessedRedirect> _processedRedirects;
        private readonly List<IResult> _results;
        private List<IProcessor> _activeProcessors;

        public RedirectEngine(
            IConfiguration configuration,
            IUrlHelper urlHelper,
            IUrlParser urlParser,
			IUrlFormatter urlFormatter,
            IRedirectParser redirectParser,
            IHttpClient httpClient)
        {
            _configuration = configuration;
            _urlHelper = urlHelper;
            _urlParser = urlParser;
            _redirectParser = redirectParser;
            _httpClient = httpClient;
            Processors = new List<IProcessor>
            {
                new InvalidProcessor(),
                new IdenticalProcessor(
                    _urlHelper),
                new DuplicateProcessor(
                    _configuration,
                    _urlHelper),
                new ExcludeProcessor(
                    _configuration),
                new RedirectProcessor(
                    _configuration,
                    _urlHelper,
                    _httpClient,
                    _urlParser,
					urlFormatter,
					new RedirectHelper(
						_configuration,
						_urlParser,
						urlFormatter)
					)
            };
            _redirects = new List<IRedirect>();
            _parsedRedirects = new List<IParsedRedirect>();
            _processedRedirects = new List<IProcessedRedirect>();
            _results = new List<IResult>();
            _activeProcessors = new List<IProcessor>();

            Exporters = new List<IExporter>
            {
                new WebConfigExporter(
                    _configuration,
                    _urlParser,
                    urlFormatter),
                new AwsS3StaticWebsiteExporter(
                    _configuration,
                    _urlParser,
                    urlFormatter)
            };
        }

        public event EventHandler<RedirectProcessedEventArgs> RedirectProcessed;

        protected virtual void OnRedirectProcessed(
            RedirectProcessedEventArgs e)
        {
            RedirectProcessed?.Invoke(this, e);
        }

        public IList<IProcessor> Processors { get; }
        public IList<IExporter> Exporters { get; }

        public IRedirectProcessingResult Run()
        {
            var startTime = DateTime.UtcNow;

            LoadRedirectsFromCsvFiles();
            ParseRedirects();

            switch(_configuration.Mode)
            {
                case Mode.Process:
                    ActiveProcessors();
                    PreloadParsedRedirects();
                    ProcessParsedRedirects();
                    CollectResults();
                    break;
                case Mode.Export:
                    Export();
                    break;
            }

            var endTime = DateTime.UtcNow;

            return new RedirectProcessingResult
            {
                Processors = _activeProcessors,
                Redirects = _redirects,
                ParsedRedirects = _parsedRedirects,
                ProcessedRedirects = _processedRedirects,
                Results = _results,
                StartTime = startTime,
                EndTime = endTime
            };
        }

        private void Export()
        {
            if (string.IsNullOrWhiteSpace(
                _configuration.Exporter))
            {
                throw new Exception($"exporter '{_configuration.Exporter}' not found");
            }

            var exporter = Exporters.FirstOrDefault(
                x => x.Name.Equals(
                    _configuration.Exporter,
                    StringComparison.InvariantCultureIgnoreCase));

            if (exporter == null)
            {
                throw new Exception($"exporter '{_configuration.Exporter}' not found");
            }

            exporter.Export(
                _redirects,
                _configuration.OutputDir);
        }

        private void ActiveProcessors()
        {
            _activeProcessors = (_configuration.Processors.Any()
                ? Processors.Where(p => _configuration.Processors.Contains(
                    p.Name, StringComparer.OrdinalIgnoreCase))
                : Processors).ToList();
        }

        private void LoadRedirectsFromCsvFiles()
        {
            _redirects.Clear();

            foreach (var redirectsCsvFile in _configuration.RedirectCsvFiles)
            {
                using (var redirectCsvReader = 
                    new RedirectCsvReader(
                        _configuration,
                        redirectsCsvFile))
                {
                    _redirects.AddRange(
                        redirectCsvReader.ReadAllRedirects());
                }
            }
        }

        private void ParseRedirects()
        {
            _parsedRedirects.Clear();

            foreach(var redirect in _redirects.ToList())
            {
                _parsedRedirects.Add(
                    _redirectParser.ParseRedirect(redirect));
            }

            _parsedRedirects.Sort();
        }

        private void PreloadParsedRedirects()
        {
            foreach (var processor in Processors
                .OfType<IProcessorPreload>()
                .ToList())
            {
                processor.PreloadParsedRedirects(
                    _parsedRedirects);
            }
        }

        private void ProcessParsedRedirects()
        {
            _processedRedirects.Clear();

            var parsedRedirects = (_configuration.SampleCount > 0
                ? _parsedRedirects.Take(_configuration.SampleCount)
                : _parsedRedirects).ToList();

            foreach (var parsedRedirect in parsedRedirects)
            {
                var processedRedirect = new ProcessedRedirect
                {
                    ParsedRedirect = parsedRedirect
                };

                _processedRedirects.Add(processedRedirect);

                try
                {
                    foreach (var processor in _activeProcessors)
                    {
                        processor.Process(processedRedirect);
                    }
                }
                catch (Exception e)
                {
                    processedRedirect.Results.Add(
                        new Result
                        {
                            Type = ResultTypes.UnknownErrorResult,
                            Message = string.Format(
                                "Unknown error '{0}'!",
                                e.Message)
                        });
                }

                // raise processed redirect event
                OnRedirectProcessed(
                    new RedirectProcessedEventArgs(
                        processedRedirect));
            }
        }

        private void CollectResults()
        {
            foreach (var processor in Processors)
            {
                _results.AddRange(
                    processor.Results);
            }
        }
    }
}