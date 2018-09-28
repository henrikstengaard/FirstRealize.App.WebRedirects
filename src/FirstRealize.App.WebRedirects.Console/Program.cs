using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Exporters;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Readers;
using FirstRealize.App.WebRedirects.Core.Reports;
using FirstRealize.App.WebRedirects.Core.Validators;
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
            // write webredirects title
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.WriteLine(
                string.Format(
                    "FirstRealize App WebRedirects v{0}",
                    Assembly.GetExecutingAssembly().GetName().Version));
            System.Console.WriteLine(string.Empty);

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

            // write progessing redirects
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Reading configuration file '{0}'",
                    configurationFile));

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

            // write read configuration file done
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            var outputDir = Path.GetDirectoryName(
                configurationFile);

            // TODO: Apply if needed
            //ServicePointManager.SecurityProtocol = 
            //	SecurityProtocolType.Tls12;

            // create http client depending use test http client
            IHttpClient httpClient;
            if (configuration.UseTestHttpClient)
            {
                httpClient = new TestHttpClient();
            }
            else
            {
                httpClient = new HttpClient();
            }

            // create redirect engine
            var urlParser = new UrlParser(
                configuration);
			var urlFormatter = new UrlFormatter();
            var urlHelper = new UrlHelper(
                configuration,
				urlParser,
				urlFormatter);
            var redirectParser = new RedirectParser(
                configuration,
                urlParser);
            var redirectEngine = new RedirectEngine(
                configuration,
                urlHelper,
                urlParser,
                redirectParser,
                httpClient);

            // create processed redirect validator
            var processedRedirectValidator =
                new ProcessedRedirectValidator(
                    configuration,
                    urlHelper);

            // handle processed redirect event to show progress
            redirectEngine.RedirectProcessed += (o, e) =>
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                var result = ".";
                if (e.ProcessedRedirect.Results.Any(
                    r => r.Type.Equals(ResultTypes.UnknownErrorResult)))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    result = "X";
                }
                else if (e.ProcessedRedirect.Results.Any(
                    r => r.Type.Equals(ResultTypes.ExcludedRedirect)))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    result = "%";
                }
                else if (e.ProcessedRedirect.Results.Any(
                    r => r.Type.Equals(ResultTypes.InvalidResult)))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    result = "?";
                }
                else if (e.ProcessedRedirect.Results.Any(
                    r => r.Type.Equals(ResultTypes.IdenticalResult)))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    result = "=";
                }
                else if (e.ProcessedRedirect.Results.Any(
                    r => r.Type.Equals(ResultTypes.CyclicRedirect)))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    result = "O";
                }
                else if (e.ProcessedRedirect.Results.Any(
                    r => r.Type.Equals(ResultTypes.TooManyRedirects)))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    result = "*";
                }
                else
                {
                    var urlResponseResult = e
                    .ProcessedRedirect
                    .Results
                    .OfType<UrlResponseResult>()
                    .FirstOrDefault(r => r.Type.Equals(ResultTypes.UrlResponse));
                    if (urlResponseResult != null && !urlHelper.AreIdentical(
                        e.ProcessedRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri,
                        urlResponseResult.Url))
                    {
                        System.Console.ForegroundColor = ConsoleColor.Yellow;
                        result = "!";
                    }
                }

                System.Console.Write(result);
            };

            // run redirect engine
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Processing redirects");

            var redirectProcessingResult = 
                redirectEngine.Run();

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            if (configuration.Export)
            {
                var webConfigExporter = new WebConfigExporter(
                    configuration,
                    urlParser,
                    urlFormatter);

                webConfigExporter.Export(
                    redirectProcessingResult.Redirects,
                    outputDir);
            }
            else
            {
                Reports(
                    outputDir,
                    redirectProcessingResult,
                    processedRedirectValidator);
            }

            return 0;
        }

        static void Reports(
            string outputDir,
            IRedirectProcessingResult redirectProcessingResult,
            IProcessedRedirectValidator processedRedirectValidator)
        {
            // create and write redirect summary report
            // ----------------------------------------
            var redirectSummaryReportCsvFile = Path.Combine(
                outputDir,
                "redirect_summary.csv");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Building and writing redirect summary report file '{0}'",
                    redirectSummaryReportCsvFile));

            var redirectSummaryReport =
                new RedirectSummaryReport(
                    processedRedirectValidator);
            redirectSummaryReport.Build(
                redirectProcessingResult);
            redirectSummaryReport.WriteReportCsvFile(
                redirectSummaryReportCsvFile);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            // create and write old url domain report
            // --------------------------------------
            var oldUrlDomainReportCsvFile = Path.Combine(
                outputDir,
                "oldurl_domains.csv");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Building and writing old url domains report file '{0}'",
                    oldUrlDomainReportCsvFile));

            var oldUrlDomainReport =
                new OldUrlDomainReport();
            oldUrlDomainReport.Build(
                redirectProcessingResult);
            oldUrlDomainReport.WriteReportCsvFile(
                oldUrlDomainReportCsvFile);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            // create and write new url domain report
            // --------------------------------------
            var newUrlDomainReportCsvFile = Path.Combine(
                outputDir,
                "newurl_domains.csv");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Building and writing new url domains report file '{0}'",
                    newUrlDomainReportCsvFile));

            var newUrlDomainReport =
                new NewUrlDomainReport();
            newUrlDomainReport.Build(
                redirectProcessingResult);
            newUrlDomainReport.WriteReportCsvFile(
                newUrlDomainReportCsvFile);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            // create and write processed redirect report
            // ------------------------------------------
            var processedRedirectReportCsvFile = Path.Combine(
                outputDir,
                "processed_redirects.csv");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Building and writing processed redirects report file '{0}'",
                    processedRedirectReportCsvFile));

            var processedRedirectReport =
                new ProcessedRedirectReport();
            processedRedirectReport.Build(
                redirectProcessingResult);
            processedRedirectReport.WriteReportCsvFile(
                processedRedirectReportCsvFile);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            // create and write output redirect report
            // ---------------------------------------
            var outputRedirectReportCsvFile = Path.Combine(
                outputDir,
                "output_redirects.csv");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Building and writing output redirects report file '{0}'",
                    outputRedirectReportCsvFile));

            var outputRedirectReport =
                new OutputRedirectReport(
                    processedRedirectValidator,
                    false);
            outputRedirectReport.Build(
                redirectProcessingResult);
            outputRedirectReport.WriteReportCsvFile(
                outputRedirectReportCsvFile);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.WriteLine(string.Empty);

            // create and write output redirect including not matching new url report
            // ----------------------------------------------------------------------
            var outputRedirectIncludingNotMatchingNewUrlReportCsvFile = Path.Combine(
                outputDir,
                "output_redirects_incl_not_matching.csv");

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(
                string.Format(
                    "Building and writing output redirects including not matching new url report file '{0}'",
                    outputRedirectReportCsvFile));

            var outputRedirectIncludingNotMatchingNewUrlReport =
                new OutputRedirectReport(
                    processedRedirectValidator,
                    true);
            outputRedirectIncludingNotMatchingNewUrlReport.Build(
                redirectProcessingResult);
            outputRedirectIncludingNotMatchingNewUrlReport.WriteReportCsvFile(
                outputRedirectIncludingNotMatchingNewUrlReportCsvFile);

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Done");
            System.Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void Usage()
        {
            var consoleFilename = Path.GetFileName(
                Assembly.GetExecutingAssembly().CodeBase);
            System.Console.WriteLine(
                string.Join(Environment.NewLine, new[]
                {
                    string.Format("Usage: {0}", consoleFilename),
                    "  -c|--config \"configuration.json\""
                }));
        }
    }
}