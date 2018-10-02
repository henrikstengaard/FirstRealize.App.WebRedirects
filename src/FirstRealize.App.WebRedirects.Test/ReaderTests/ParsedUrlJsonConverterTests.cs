using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class ParsedUrlJsonConverterTests
    {
        [Test]
        public void ConvertJsonToParsedUrl()
        {
            var parsedUrlJsonConverter = new ParsedUrlJsonConverter();

            Assert.AreEqual(
                true,
                parsedUrlJsonConverter.CanConvert(typeof(ParsedUrl)));
            Assert.AreEqual(
                false,
                parsedUrlJsonConverter.CanConvert(typeof(DateTime)));

            // read parsed urls from json
            var parsedUrls = new List<object>();
            var json = @"{
    'Test1': 'http://www.test.local',
    'Test2': 'not-a-valid'
}";
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.String &&
                        reader.Value != null)
                    {
                        parsedUrls.Add(parsedUrlJsonConverter.ReadJson(
                            reader,
                            typeof(ParsedUrl),
                            reader.Value,
                            new JsonSerializer()));

                    }
                }
            }

            // verify parsed urls
            Assert.AreEqual(2, parsedUrls.Count);
            var parsedUrl1 = parsedUrls[0] as IParsedUrl;
            Assert.IsNotNull(parsedUrl1);
            Assert.AreEqual(
                "http",
                parsedUrl1.Scheme);
            Assert.AreEqual(
                "www.test.local",
                parsedUrl1.Host);
            Assert.AreEqual(
                80,
                parsedUrl1.Port);
            var parsedUrl2 = parsedUrls[1] as IParsedUrl;
            Assert.AreNotEqual(
                null,
                parsedUrl2);
            Assert.AreEqual(
                false,
                parsedUrl2.IsValid);
        }
    }
}