using Newtonsoft.Json;
using System;

namespace FirstRealize.App.WebRedirects.Core.Configuration
{
    public class UriJsonConverter : JsonConverter
    {
        public override bool CanConvert(
            Type objectType)
        {
            return (objectType == typeof(Uri));
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            Uri uri;
            if (!Uri.TryCreate(reader.Value != null ? reader.Value.ToString() : string.Empty, UriKind.Absolute, out uri))
            {
                return null;
            }

            return uri;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            Uri uri = (Uri)value;
            writer.WriteValue(uri.AbsoluteUri);
        }
    }
}