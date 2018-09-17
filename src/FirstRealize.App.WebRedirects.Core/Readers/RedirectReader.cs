using CsvHelper;
using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace FirstRealize.App.WebRedirects.Core.Readers
{
    public class RedirectReader
    {
        private readonly string _path;

        public RedirectReader(string path)
        {
            _path = path;
        }

        public IEnumerable<Redirect> ReadAllRedirects()
        {
            using (var csvReader = new CsvReader(
                new StreamReader(_path), new CsvHelper.Configuration.Configuration
                {
                    Delimiter = ";",
                    PrepareHeaderForMatch = header => header.ToLower()
                }, false))
            {
                if (csvReader.Read())
                {
                    csvReader.ReadHeader();
                }
                while (csvReader.Read())
                {
                    yield return new Redirect
                    {
                        OldUrl = new Url
                        {
                            Raw = csvReader["oldurl"]
                        },
                        NewUrl = new Url
                        {
                            Raw = csvReader["newurl"]
                        }
                    };
                }
            }
        }
    }
}