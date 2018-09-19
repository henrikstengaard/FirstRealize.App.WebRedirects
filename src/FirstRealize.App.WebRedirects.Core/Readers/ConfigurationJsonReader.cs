using FirstRealize.App.WebRedirects.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Readers
{
    public class ConfigurationJsonReader : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IConfiguration ReadConfiguationFile(
            string configurationFile)
        {
            var json = File.ReadAllText(configurationFile);
            var configuration = JsonConvert
                .DeserializeObject<Configuration.Configuration>(
                json);

            var configurationDir = 
                Path.GetDirectoryName(configurationFile);

            var redirectCsvFiles = configuration.RedirectCsvFiles
                .ToList();

            for (int i = 0; i < redirectCsvFiles.Count; i++)
            {
                // absolute path
                if (Path.IsPathRooted(redirectCsvFiles[i]))
                {
                    continue;
                }

                // make relative path absolute
                redirectCsvFiles[i] = Path.Combine(configurationDir, redirectCsvFiles[i]);

                // throw file not found exception, if file doesn't exist
                if (!File.Exists(redirectCsvFiles[i]))
                {
                    throw new FileNotFoundException(
                        string.Format(
                            "Redirect csv file '{0}' is not found",
                            redirectCsvFiles[i]));
                }
            }

            configuration.RedirectCsvFiles = redirectCsvFiles;

            return configuration;
        }
    }
}