using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using FirstRealize.App.WebRedirects.Core.Readers;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectEngine
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlParser _urlParser;
        private readonly IRedirectParser _redirectParser;
        private readonly IHttpClient _httpClient;
        private readonly IList<IProcessor> _processors;

        private List<Redirect> _redirects;
        private List<ProcessedRedirect> _processedRedirects;
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

            _redirects = new List<Redirect>();
            _processedRedirects = new List<ProcessedRedirect>();
            _results = new List<IResult>();
        }

        public IEnumerable<Redirect> Redirects
        {
            get
            {
                return _redirects;
            }
        }

        public IEnumerable<IProcessedRedirect> ProcessedRedirects
        {
            get
            {
                return _processedRedirects;
            }
        }

        public IEnumerable<IResult> Results
        {
            get
            {
                return _results;
            }
        }

        public void Run()
        {
            LoadRedirectsFromCsvFiles();
            ParseRedirects();
            PreloadRedirects();
            ProcessRedirects();
            CollectResults();
        }

        private void LoadRedirectsFromCsvFiles()
        {
            _redirects = new List<Redirect>();

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
            foreach(var redirect in _redirects.ToList())
            {
                _redirectParser.ParseRedirect(redirect);
            }

            _redirects.Sort();
        }

        private void PreloadRedirects()
        {
            foreach (var processor in _processors
                .OfType<IProcessorPreload>()
                .ToList())
            {
                processor.PreloadRedirects(_redirects);
            }
        }

        private void ProcessRedirects()
        {
            _processedRedirects = new List<ProcessedRedirect>();

            foreach (var redirect in _redirects.ToList())
            {
                var processedRedirect = new ProcessedRedirect
                {
                    Redirect = redirect
                };

                _processedRedirects.Add(processedRedirect);

                if (!redirect.IsValid ||
                    redirect.IsIdentical)
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