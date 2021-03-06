﻿using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
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

        public static ParsedUrl DefaultHost =
            new ParsedUrl
            {
                Scheme = "http",
                Host = "www.test.local",
                Port = 80
            };

        public static Configuration DefaultConfiguration =
            new Configuration
            {
                DefaultUrl = DefaultHost,
                ForceHttpHostPatterns = new[]
                {
                    "www\\.test\\.local"
                },
                MaxRedirectCount = 10
            };

        public static IEnumerable<IRedirect> GetRedirects()
        {
            return new List<IRedirect>
            {
                new Redirect
                {
                    OldUrl = "/example/path",
                    NewUrl = "/new-url"
                },
                new Redirect
                {
                    OldUrl = "/new-url",
                    NewUrl = "/another/path"
                },
                // causes duplicate redirect
                new Redirect
                {
                    OldUrl = "/new-url",
                    NewUrl = "/redirect/somwhere/else"
                },
                // causes cyclic redirect for redirect from old url '/example/path' to '/new-url'
                new Redirect
                {
                    OldUrl = "/another/path",
                    NewUrl = "/example/path"
                }
            };
        }

        public static IEnumerable<IParsedRedirect> GetParsedRedirects()
        {
            return GetParsedRedirects(
                DefaultConfiguration);
        }

        public static IEnumerable<IParsedRedirect> GetParsedRedirects(
            IConfiguration configuration)
        {
            return GetParsedRedirects(
                configuration,
                GetRedirects());
        }

        public static IEnumerable<IParsedRedirect> GetParsedRedirects(
            IEnumerable<IRedirect> redirects)
        {
            return GetParsedRedirects(
                DefaultConfiguration,
                redirects);
        }

        public static IEnumerable<IParsedRedirect> GetParsedRedirects(
            IConfiguration configuration,
            IEnumerable<IRedirect> redirects)
        {
            var redirectParser = new RedirectParser(
                configuration,
                new UrlParser(),
                new UrlFormatter());

            var parsedRedirects = new List<IParsedRedirect>();

            foreach (var redirect in redirects.ToList())
            {
                parsedRedirects.Add(
                    redirectParser.ParseRedirect(
                        redirect));
            }

            return parsedRedirects;
        }

        public static IEnumerable<IProcessedRedirect> GetProcessedRedirects(
            IEnumerable<IProcessor> processors)
        {
            var configuration = DefaultConfiguration;
            return GetProcessedRedirects(
                GetParsedRedirects(
                    configuration),
                processors);
        }

        public static IEnumerable<IProcessedRedirect> GetProcessedRedirects(
            IConfiguration configuration,
            IEnumerable<IProcessor> processors)
        {
            return GetProcessedRedirects(
                GetParsedRedirects(
                    configuration),
                processors);
        }

        public static IEnumerable<IProcessedRedirect> GetProcessedRedirects(
            IEnumerable<IParsedRedirect> parsedRedirects,
            IEnumerable<IProcessor> processors)
        {
            var redirectsList = parsedRedirects.ToList();
            var processorsList = processors.ToList();

            var processedRedirects =
                new List<IProcessedRedirect>();

            foreach (var redirect in redirectsList)
            {
                var processedRedirect = new ProcessedRedirect
                {
                    ParsedRedirect = redirect
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