using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FirstRealize.App.WebRedirects.Test.TestData
{
    class TestData
    {
        public static string CurrentDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static Uri DefaultHost =
            new Uri("http://www.test.local");

        public static IConfiguration DefaultConfiguration =
            new Configuration
            {
                DefaultOldUrl = DefaultHost,
                DefaultNewUrl = DefaultHost,
                ForceHttp = true
            };

        public static IEnumerable<Redirect> GetRedirects()
        {
            return new List<Redirect>
            {
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/example/path"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/new-url"
                    }
                },
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/new-url"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/another/path"
                    }
                },
                // causes duplicate redirect
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/new-url"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/redirect/somwhere/else"
                    }
                },
                // causes cyclic redirect for redirect from old url '/example/path' to '/new-url'
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/another/path"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/example/path"
                    }
                }
            };
        }

        public static IEnumerable<Redirect> GetParsedRedirects()
        {
            var redirectParser = new RedirectParser(
                DefaultConfiguration,
                new UrlParser());

            var parsedRedirects = new List<Redirect>();

            foreach (var redirect in GetRedirects())
            {
                redirectParser.ParseRedirect(
                    redirect);

                parsedRedirects.Add(redirect);
            }

            return parsedRedirects;
        }

        public static IEnumerable<IProcessedRedirect> GetProcessedRedirects(
            IEnumerable<IProcessor> processors)
        {
            return GetProcessedRedirects(
                GetParsedRedirects(),
                processors);
        }

        public static IEnumerable<IProcessedRedirect> GetProcessedRedirects(
            IEnumerable<Redirect> redirects,
            IEnumerable<IProcessor> processors)
        {
            var redirectsList = redirects.ToList();
            var processorsList = processors.ToList();

            var processedRedirects =
                new List<IProcessedRedirect>();

            foreach (var redirect in redirectsList)
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