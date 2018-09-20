using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Readers;
using FirstRealize.App.WebRedirects.Core.Reports;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

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

            // expand environment variables in arguments
            args = args.Select(x =>
                Environment.ExpandEnvironmentVariables(x))
                .ToArray();

            var argumentParser =
                new ArgumentParser(args);

            // parse arguments
            string configurationFile = argumentParser
                .ParseArgumentValue("^(-c|--config)");
            bool process = argumentParser
                .ParseArgumentSwitch("^(-p|--process)");

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

            var outputDir = Path.GetDirectoryName(
                configurationFile);

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
            var redirectProcessingResult = 
                redirectEngine.Run(process);

            // create and write reports
            var oldUrlDomainReport =
                new OldUrlDomainReport();
            oldUrlDomainReport.Build(
                redirectProcessingResult);
            var oldUrlDomainReportCsvFile = Path.Combine(
                outputDir,
                "oldurl_domains.csv");
            oldUrlDomainReport.WriteReportCsvFile(
                oldUrlDomainReportCsvFile);

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