using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ValidatorTests
{
    [TestFixture]
    public class ProcessedRedirectValidatorTests
    {
        private readonly IEnumerable<IProcessedRedirect> _processedRedirects;

        public ProcessedRedirectValidatorTests()
        {
            _processedRedirects = new[]
            {
                // processed redirect with cyclic redirect must be considered invalid
                new ProcessedRedirect
                {
                    ParsedRedirect = new ParsedRedirect
                    {
                        OldUrl = new Url
                        {
                            Parsed = new Uri("http://www.test1.local/url1")
                        },
                        NewUrl = new Url
                        {
                            Parsed = new Uri("http://www.test3.local/url8")
                        }
                    },
                    Results = new[]
                    {
                        new Result
                        {
                            Type = ResultTypes.CyclicRedirect
                        }
                    }
                },
                // processed redirect with not matching new url and url response status code 404 must be considered invalid
                new ProcessedRedirect
                {
                    ParsedRedirect = new ParsedRedirect
                    {
                        OldUrl = new Url
                        {
                            Parsed = new Uri("http://www.test1.local/url2")
                        },
                        NewUrl = new Url
                        {
                            Parsed = new Uri("http://www.test2.local/url5")
                        }
                    },
                    Results = new[]
                    {
                        new UrlResponseResult
                        {
                            Type = ResultTypes.UrlResponse,
                            Url = "http://www.test2.local/url9",
                            StatusCode = 404
                        }
                    }
                },
                // processed redirect with matching new url and url response status code 404 must be considered invalid
                new ProcessedRedirect
                {
                    ParsedRedirect = new ParsedRedirect
                    {
                        OldUrl = new Url
                        {
                            Parsed = new Uri("http://www.test1.local/url2")
                        },
                        NewUrl = new Url
                        {
                            Parsed = new Uri("http://www.test2.local/url5")
                        }
                    },
                    Results = new[]
                    {
                        new UrlResponseResult
                        {
                            Type = ResultTypes.UrlResponse,
                            Url = "http://www.test2.local/url5",
                            StatusCode = 404
                        }
                    }
                },
                // processed redirect with matching new url and url response status code 200 must be considered valid
                new ProcessedRedirect
                {
                    ParsedRedirect = new ParsedRedirect
                    {
                        OldUrl = new Url
                        {
                            Parsed = new Uri("http://www.test2.local/url3")
                        },
                        NewUrl = new Url
                        {
                            Parsed = new Uri("http://www.test2.local/url9")
                        }
                    },
                    Results = new[]
                    {
                        new UrlResponseResult
                        {
                            Type = ResultTypes.UrlResponse,
                            Url = "http://www.test2.local/url9",
                            StatusCode = 200
                        }
                    }
                },
                // processed redirect with not matching new url must be considered
                // valid or invalid depending on configuration
                new ProcessedRedirect
                {
                    ParsedRedirect = new ParsedRedirect
                    {
                        OldUrl = new Url
                        {
                            Parsed = new Uri("http://www.test2.local/url4")
                        },
                        NewUrl = new Url
                        {
                            Parsed = new Uri("http://www.test2.local/url9")
                        }
                    },
                    Results = new[]
                    {
                        new UrlResponseResult
                        {
                            Type = ResultTypes.UrlResponse,
                            Url = "http://www.test2.local/url10",
                            StatusCode = 200
                        }
                    }
                }
            };
        }

        [Test]
        public void ValidateProcessedRedirects()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
			var urlFormatter = new UrlFormatter();
			var urlParser = new UrlParser(
				configuration);
            var urlHelper = new UrlHelper(
                configuration,
				urlParser,
				urlFormatter);
            var processedRediretValidator =
                new ProcessedRedirectValidator(
                    urlHelper);

            var validProcessedRedirects = _processedRedirects
                .Where(x => processedRediretValidator.IsValid(x, true))
                .ToList();

            Assert.AreEqual(
                2,
                validProcessedRedirects.Count);

            var urlResponseResult1 = validProcessedRedirects[0]
                .Results
                .OfType<UrlResponseResult>()
                .FirstOrDefault(r => r.Url != null && r.Url != null);
            Assert.AreEqual(
                "http://www.test2.local/url3",
                validProcessedRedirects[0].ParsedRedirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test2.local/url9",
                urlResponseResult1.Url);

            var urlResponseResult2 = validProcessedRedirects[1]
                .Results
                .OfType<UrlResponseResult>()
                .FirstOrDefault(r => r.Url != null && r.Url != null);
            Assert.IsNotNull(urlResponseResult2);
            Assert.AreEqual(
                "http://www.test2.local/url4",
                validProcessedRedirects[1].ParsedRedirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test2.local/url10",
                urlResponseResult2.Url);
        }

        [Test]
        public void ValidateProcessedRedirectsExcludingNotMatchingNewUrls()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
			var urlFormatter = new UrlFormatter();
			var urlParser = new UrlParser(
				configuration);
			var urlHelper = new UrlHelper(
				configuration,
				urlParser,
				urlFormatter);
			var processedRediretValidator =
                new ProcessedRedirectValidator(
                    urlHelper);

            var validProcessedRedirects = _processedRedirects
                .Where(x => processedRediretValidator.IsValid(x, false))
                .ToList();

            Assert.AreEqual(
                1,
                validProcessedRedirects.Count);

            var urlResponseResult1 = validProcessedRedirects[0]
                .Results
                .OfType<UrlResponseResult>()
                .FirstOrDefault(r => r.Url != null && r.Url != null);
            Assert.AreEqual(
                "http://www.test2.local/url3",
                validProcessedRedirects[0].ParsedRedirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test2.local/url9",
                urlResponseResult1.Url);
        }
    }
}