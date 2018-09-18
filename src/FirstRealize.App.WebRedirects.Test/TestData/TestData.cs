using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using FirstRealize.App.WebRedirects.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.TestData
{
    class TestData
    {
        public static Uri DefaultHost =
            new Uri("http://www.test.local");

        public static IConfiguration DefaultConfiguration =
            new Configuration
            {
                DefaultOldUrl = DefaultHost,
                DefaultNewUrl = DefaultHost,
                ForceHttp = true
            };

        public static IEnumerable<RawRedirect> GetRawRedirects()
        {
            return new List<RawRedirect>
            {
                new RawRedirect
                {
                    OldUrl = "/example/path",
                    NewUrl = "/new-url"
                },
                new RawRedirect
                {
                    OldUrl = "/new-url",
                    NewUrl = "/another/path"
                },
                // causes duplicate redirect
                new RawRedirect
                {
                    OldUrl = "/new-url",
                    NewUrl = "/redirect/somwhere/else"
                },
                // causes cyclic redirect for redirect from old url '/example/path' to '/new-url'
                new RawRedirect
                {
                    OldUrl = "/another/path",
                    NewUrl = "/example/path"
                }
            };
        }

        public static IEnumerable<Redirect> GetParsedRedirects()
        {
            var redirectParser = new RedirectParser(
                DefaultConfiguration,
                new UrlParser());

            var parsedRedirects = new List<Redirect>();

            foreach (var rawRedirect in GetRawRedirects())
            {
                parsedRedirects.Add(
                    redirectParser.ParseRedirect(
                        rawRedirect.OldUrl,
                        rawRedirect.NewUrl));
            }

            return parsedRedirects;
        }

        public static IEnumerable<IProcessedRedirect> GetProcessedRedirects(
            IEnumerable<IProcessor> processors)
        {
            var processorsList = processors.ToList();

            var processedRedirects =
                new List<IProcessedRedirect>();

            foreach (var redirect in GetParsedRedirects())
            {
                var processedRedirect = new ProcessedRedirect
                {
                    Redirect = redirect
                };

                foreach(var processor in processors)
                {
                    processor.Process(processedRedirect);
                }

                processedRedirects.Add(processedRedirect);
            }

            return processedRedirects;
        }
    }
}