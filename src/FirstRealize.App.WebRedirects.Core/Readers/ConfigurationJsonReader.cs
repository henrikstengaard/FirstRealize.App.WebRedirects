using FirstRealize.App.WebRedirects.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FirstRealize.App.WebRedirects.Core.Readers
{
    public class ConfigurationJsonReader : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IConfiguration ReadConfiguationFile(
            string path)
        {
            return ReadConfiguationJson(
                File.ReadAllText(path));
        }

        public IConfiguration ReadConfiguationJson(
            string json)
        {
            return JsonConvert.DeserializeObject<Configuration.Configuration>(
                json);
        }
    }
}