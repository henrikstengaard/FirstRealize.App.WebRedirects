using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class RedirectProcessor
    {
        private readonly Configuration _configuration;
        private readonly IList<IProcessor> _processors;

        public RedirectProcessor(
            Configuration configuration)
        {
            _configuration = configuration;
            _processors = new List<IProcessor>
            {
                new ExcludeProcessor(_configuration),
                new DuplicateProcessor(),
                new CyclicProcessor(_configuration)
            };
        }

        public void Preload(IEnumerable<Redirect> redirects)
        {
            foreach(var processor in _processors
                .OfType<IProcessorPreload>()
                .ToList())
            {
                processor.Preload(redirects);
            }
        }

        public IProcessedRedirect Process(
            Redirect redirect)
        {
            var processedRedirect = new ProcessedRedirect
            {
                Redirect = redirect
            };

            if (!redirect.IsValid ||
                redirect.IsIdentical)
            {
                return processedRedirect;
            }

            foreach(var processor in _processors)
            {
                processor.Process(processedRedirect);
            }

            return processedRedirect;
        }
    }
}