using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.HelperTests
{
	[TestFixture]
	public class RedirectHelperTests
	{
		private readonly IRedirectHelper _redirectHelper;
		private readonly IList<IParsedRedirect> _replaceRedirects;

		public RedirectHelperTests()
		{
			var configuration =
				TestData.TestData.DefaultConfiguration;
			_redirectHelper = new RedirectHelper(
				TestData.TestData.DefaultConfiguration,
				new UrlParser(),
				new UrlFormatter());
			_replaceRedirects = TestData.TestData.GetParsedRedirects(
				TestData.TestData.DefaultConfiguration,
				new[]
				{
					new Redirect
					{
						OldUrl = "/url",
						NewUrl = "/replaced",
						RedirectType = RedirectType.Replace
					}
				})
				.ToList();
		}

		[Test]
		public void ReplacedUrlWithReplaceRedirect()
		{
			var replacedUrl = _redirectHelper.Replace(
				"http://www.test.local/url/to/somewhere",
				_replaceRedirects[0]);

			Assert.IsNotNull(replacedUrl);
			Assert.AreEqual(
				"http://www.test.local/replaced/to/somewhere",
				replacedUrl);
		}

		[Test]
		public void ReplacedUrlAndQueryStringWithReplaceRedirect()
		{
			var replacedUrl = _redirectHelper.Replace(
				"http://www.test.local/url/to/somewhere?query=test",
				_replaceRedirects[0]);

			Assert.IsNotNull(replacedUrl);
			Assert.AreEqual(
				"http://www.test.local/replaced/to/somewhere?query=test",
				replacedUrl);
		}
	}
}