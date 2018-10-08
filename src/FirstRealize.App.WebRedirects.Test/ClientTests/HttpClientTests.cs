using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.ClientTests
{
    [TestFixture]
    public class HttpClientTests
    {
        [Test]
        public void GetGoogleUrlHttps()
        {
            var httpClient = new HttpClient(
                TestData.TestData.DefaultConfiguration,
                new UrlParser());

            var response = httpClient.Get("https://www.google.com");

            Assert.IsNotNull(
                response);
            Assert.AreEqual(
                200,
                response.StatusCode);
        }

        [Test]
        public void GetGoogleUrlHttp()
        {
            var httpClient = new HttpClient(
                TestData.TestData.DefaultConfiguration,
                new UrlParser());

            var response = httpClient.Get("http://www.google.com");

            Assert.IsNotNull(
                response);
            Assert.AreEqual(
                200,
                response.StatusCode);
        }

        [Test]
        public void GetInvalidUrlThrowsException()
        {
            var httpClient = new HttpClient(
                TestData.TestData.DefaultConfiguration,
                new UrlParser());

            Assert.Throws<HttpException>(
                () => httpClient.Get("/invalid-url"));
        }

        [Test]
        public void ZeroTimeoutThrowsException()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            configuration.HttpClientTimeout = 0;
            var httpClient = new HttpClient(
                configuration,
                new UrlParser());

            Assert.Throws<HttpException>(
                () => httpClient.Get("/url"));
        }
    }
}