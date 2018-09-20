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

            // TODO: Apply if needed
            //ServicePointManager.SecurityProtocol = 
            //	SecurityProtocolType.Tls12;

            // create redirect engine
            var urlParser = new UrlParser();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser);
            var redirectEngine = new RedirectEngine(
                configuration,
                urlParser,
                redirectParser,
                new HttpClient(
                    configuration)
                );

            // run redirect engine
            var redirectProcessingResult = 
                redirectEngine.Run();

            // create and write redirect summary report
            var redirectSummaryReport =
                new RedirectSummaryReport();
            redirectSummaryReport.Build(
                redirectProcessingResult);
            var redirectSummaryReportCsvFile = Path.Combine(
                outputDir,
                "redirect_summary.csv");
            redirectSummaryReport.WriteReportCsvFile(
                redirectSummaryReportCsvFile);

            // create and write old url domain report
            var oldUrlDomainReport =
                new OldUrlDomainReport();
            oldUrlDomainReport.Build(
                redirectProcessingResult);
            var oldUrlDomainReportCsvFile = Path.Combine(
                outputDir,
                "oldurl_domains.csv");
            oldUrlDomainReport.WriteReportCsvFile(
                oldUrlDomainReportCsvFile);

            // create and write new url domain report
            var newUrlDomainReport =
                new NewUrlDomainReport();
            newUrlDomainReport.Build(
                redirectProcessingResult);
            var newUrlDomainReportCsvFile = Path.Combine(
                outputDir,
                "newurl_domains.csv");
            newUrlDomainReport.WriteReportCsvFile(
                newUrlDomainReportCsvFile);

            // create and write processed redirect report
            var processedRedirectReport =
                new ProcessedRedirectReport();
            processedRedirectReport.Build(
                redirectProcessingResult);
            var processedRedirectReportCsvFile = Path.Combine(
                outputDir,
                "processed_redirects.csv");
            processedRedirectReport.WriteReportCsvFile(
                processedRedirectReportCsvFile);

            return 0;
        }

        static void Usage()
        {
            var consoleFilename = Path.GetFileName(
                Assembly.GetExecutingAssembly().CodeBase);
            System.Console.WriteLine(
                string.Join(Environment.NewLine, new[]
                {
                    string.Format("Usage: {0}", consoleFilename),
                    "  -c|--config \"configuration.json\"",
                    "  -p|--process"
                }));
        }
    }
}