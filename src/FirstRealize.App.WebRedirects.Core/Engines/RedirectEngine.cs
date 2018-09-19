using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
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
        }

        public void Run()
        {
            LoadRedirectsFromCsvFiles();
            ParseRedirects();
            PreloadRedirects();
            ProcessRedirects();
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
        }

        public void PreloadRedirects()
        {
            foreach (var processor in _processors
                .OfType<IProcessorPreload>()
                .ToList())
            {
                processor.PreloadRedirects(_redirects);
            }
        }

        public void ProcessRedirects()
        {
            //var processedRedirect = new ProcessedRedirect
            //{
            //    Redirect = redirect
            //};

            //if (!redirect.IsValid ||
            //    redirect.IsIdentical)
            //{
            //    return processedRedirect;
            //}

            //foreach (var processor in _processors)
            //{
            //    processor.Process(processedRedirect);
            //}

            //return processedRedirect;
        }

    }
}