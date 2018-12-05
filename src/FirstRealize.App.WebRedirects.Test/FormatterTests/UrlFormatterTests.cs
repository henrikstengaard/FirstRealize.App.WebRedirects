using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.FormatterTests
{
	[TestFixture]
	public class UrlFormatterTests
	{
		private readonly IUrlFormatter _urlFormatter;

		public UrlFormatterTests()
		{
			_urlFormatter = new UrlFormatter();
		}

		[Test]
		public void FormatHttpsUrl()
		{
			var parsedUrl = new ParsedUrl
			{
				Scheme = "https",
				Host = "www.test.local",
				Port = 443,
				Path = "/"
			};

			var urlFormatted = _urlFormatter.Format(
				parsedUrl);

			Assert.IsNotNull(urlFormatted);
			Assert.AreEqual(
				"https://www.test.local/",
				urlFormatted);
		}

		[Test]
		public void FormatHttpsUrlWithPortOtherThan443()
		{
			var parsedUrl = new ParsedUrl
			{
				Scheme = "https",
				Host = "www.test.local",
				Port = 444,
				Path = "/some/path"
			};

			var urlFormatted = _urlFormatter.Format(
				parsedUrl);

			Assert.IsNotNull(urlFormatted);
			Assert.AreEqual(
				"https://www.test.local:444/some/path",
				urlFormatted);
		}

		[Test]
		public void FormatHttpUrl()
		{
			var parsedUrl = new ParsedUrl
			{
				Scheme = "http",
				Host = "www.test.local",
				Port = 80,
				Path = "/"
			};

			var urlFormatted = _urlFormatter.Format(
				parsedUrl);

			Assert.IsNotNull(urlFormatted);
			Assert.AreEqual(
				"http://www.test.local/",
				urlFormatted);
		}

		[Test]
		public void FormatHttpUrlWithPortOtherThan80()
		{
			var parsedUrl = new ParsedUrl
			{
				Scheme = "http",
				Host = "www.test.local",
				Port = 8080,
				Path = "/some/path"
			};

			var urlFormatted = _urlFormatter.Format(
				parsedUrl);

			Assert.IsNotNull(urlFormatted);
			Assert.AreEqual(
				"http://www.test.local:8080/some/path",
				urlFormatted);
		}
	}
}