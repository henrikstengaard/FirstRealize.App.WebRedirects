using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using FirstRealize.App.WebRedirects.Core.Parsers;
using Newtonsoft.Json;
using System;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public class ParsedUrlJsonConverter : JsonConverter
    {
        private readonly IUrlParser _urlParser;
        private readonly IUrlFormatter _urlFormatter;

        public ParsedUrlJsonConverter()
        {
            _urlParser = new UrlParser();
            _urlFormatter = new UrlFormatter();
        }

        public override bool CanConvert(
            Type objectType)
        {
            return (objectType != null &&
                typeof(IParsedUrl).IsAssignableFrom(objectType));
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            return _urlParser.Parse(reader.Value.ToString());
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            IParsedUrl parsedUrl = (IParsedUrl)value;
            writer.WriteValue(_urlFormatter.Format(parsedUrl));
        }
    }
}