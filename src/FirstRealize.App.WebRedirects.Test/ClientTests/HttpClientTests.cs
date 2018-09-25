﻿using FirstRealize.App.WebRedirects.Core.Clients;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.ClientTests
{
    [TestFixture]
    public class HttpClientTests
    {
        [Test]
        public void GetGoogleUrlHttps()
        {
            var httpClient = new HttpClient();

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
            var httpClient = new HttpClient();

            var response = httpClient.Get("http://www.google.com");

            Assert.IsNotNull(
                response);
            Assert.AreEqual(
                200,
                response.StatusCode);
        }
    }
}