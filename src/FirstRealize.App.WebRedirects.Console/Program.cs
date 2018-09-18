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

            if (string.IsNullOrWhiteSpace(configurationFile) ||
                !File.Exists(configurationFile))
            {
                System.Console.WriteLine(
                    "ERROR: Configuration file argument is not defined");
                Usage();
                return 1;
            }

            if (File.Exists(configurationFile))
            {
                System.Console.WriteLine(
                    "ERROR: Configuration file '{0}' doesn't exist",
                    configurationFile);
                Usage();
                return 1;
            }

            // create redirect engine with path to configuration file
            // - load configuration
            // - load redirect csv files
            // - process redirects
            // - write process report, if argument is defined

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