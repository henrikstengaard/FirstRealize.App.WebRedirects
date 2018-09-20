using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using FirstRealize.App.WebRedirects.Core.Readers;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectEngine : IRedirectEngine
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;
        private readonly IRedirectParser _redirectParser;
        private readonly IHttpClient _httpClient;
        private readonly IList<IProcessor> _processors;

        private List<IRedirect> _redirects;
        private List<IParsedRedirect> _parsedRedirects;
        private List<IProcessedRedirect> _processedRedirects;
        private List<IResult> _results;

        public RedirectEngine(
            IConfiguration configuration,
            IUrlParser urlParser,
            IRedirectParser redirectParser,
            IHttpClient httpClient)
        {
            _configuration = configuration;
            _urlParser = urlParser;
            _redirectParser = redirectParser;
            _httpClient = httpClient;
            _processors = new List<IProcessor>
            {
                new ExcludeProcessor(_configuration),
                new DuplicateProcessor(),
                new RedirectProcessor(
                    _configuration,
                    httpClient,
                    _urlParser)
            };

            _redirects = new List<IRedirect>();
            _parsedRedirects = new List<IParsedRedirect>();
            _processedRedirects = new List<IProcessedRedirect>();
            _results = new List<IResult>();
        }

        public IRedirectProcessingResult Run(
            bool process)
        {
            LoadRedirectsFromCsvFiles();
            ParseRedirects();

            if (process)
            {
                PreloadParsedRedirects();
                ProcessParsedRedirects();
            }

            CollectResults();

            return new RedirectProcessingResult
            {
                Redirects = _redirects,
                ParsedRedirects = _parsedRedirects,
                ProcessedRedirects = _processedRedirects,
                Results = _results
            };
        }

        private void LoadRedirectsFromCsvFiles()
        {
            _redirects = new List<IRedirect>();

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
            _parsedRedirects = new List<IParsedRedirect>();

            foreach(var redirect in _redirects.ToList())
            {
                _parsedRedirects.Add(
                    _redirectParser.ParseRedirect(redirect));
            }

            _parsedRedirects.Sort();
        }

        private void PreloadParsedRedirects()
        {
            foreach (var processor in _processors
                .OfType<IProcessorPreload>()
                .ToList())
            {
                processor.PreloadParsedRedirects(
                    _parsedRedirects);
            }
        }

        private void ProcessParsedRedirects()
        {
            _processedRedirects = new List<IProcessedRedirect>();

            foreach (var parsedRedirect in _parsedRedirects.ToList())
            {
                var processedRedirect = new ProcessedRedirect
                {
                    ParsedRedirect = parsedRedirect
                };

                _processedRedirects.Add(processedRedirect);

                if (!parsedRedirect.IsValid ||
                    parsedRedirect.IsIdentical)
                {
                    continue;
                }

                foreach (var processor in _processors)
                {
                    processor.Process(processedRedirect);
                }
            }
        }

        private void CollectResults()
        {
            foreach (var processor in _processors)
            {
                _results.AddRange(
                    processor.Results);
            }
        }
    }
}