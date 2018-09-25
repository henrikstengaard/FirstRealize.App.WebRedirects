using FirstRealize.App.WebRedirects.Core.Clients;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.ClientTests
{
    [TestFixture]
    public class HttpClientTests
    {
        [Test]
        public void GetRequest()
        {
            var httpClient = new HttpClient();

            var response = httpClient.Get("https://www.google.com");

            Assert.IsNotNull(
                response);
            Assert.AreEqual(
                200,
                response.StatusCode);
        }
    }
}