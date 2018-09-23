using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
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
                new IdenticalProcessor(),
                new DuplicateProcessor(),
                new ExcludeProcessor(
                    _configuration),
                new RedirectProcessor(
                    _configuration,
                    _urlHelper,
                    _httpClient,
                    _urlParser)
            };
            _redirects = new List<IRedirect>();
            _parsedRedirects = new List<IParsedRedirect>();
            _processedRedirects = new List<IProcessedRedirect>();
            _results = new List<IResult>();
            _activeProcessors = new List<IProcessor>();
        }

        public IList<IProcessor> Processors { get; }

        public IRedirectProcessingResult Run()
        {
            var startTime = DateTime.UtcNow;

            ActiveProcessors();
            LoadRedirectsFromCsvFiles();
            ParseRedirects();
            PreloadParsedRedirects();
            ProcessParsedRedirects();
            CollectResults();

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
                    new RedirectCsvReader(redirectsCsvFile))
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