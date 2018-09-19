﻿using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Readers;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return 1;
            }

            string configurationFile = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (Regex.IsMatch(args[i], "^(-c|--config)", RegexOptions.IgnoreCase | RegexOptions.Compiled) 
                    && i + 1 < args.Length)
                {
                    configurationFile = args[i + 1].Trim();
                }
            }

            // write error, if configuration file argument is not defined
            if (string.IsNullOrWhiteSpace(configurationFile))
            {
                System.Console.WriteLine(
                    "ERROR: Configuration file argument is not defined");
                Usage();
                return 1;
            }

            // write error, if configuration file doesn't exist
            if (!File.Exists(configurationFile))
            {
                System.Console.WriteLine(
                    "ERROR: Configuration file '{0}' doesn't exist",
                    configurationFile);
                Usage();
                return 1;
            }

            // load configuration file
            IConfiguration configuration;
            using (var configurationJsonReader = new ConfigurationJsonReader())
            {
                configuration = configurationJsonReader
                    .ReadConfiguationFile(configurationFile);
            }

            // create redirect engine
            var urlParser = new UrlParser();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser);
            var redirectEngine = new RedirectEngine(
                configuration,
                urlParser,
                redirectParser,
                new HttpClient()
                );

            // run redirect engine
            redirectEngine.Run();

            // write processed redirects report


            return 0;
        }

        static void Usage()
        {
            var consoleFilename = Path.GetFileName(
                Assembly.GetExecutingAssembly().CodeBase);
            System.Console.WriteLine("Usage: {0} -c \"configuration.json\"", consoleFilename);
        }
    }
}