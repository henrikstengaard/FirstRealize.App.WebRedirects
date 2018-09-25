﻿using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class RedirectProcessor : IProcessor, IProcessorPreload
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpClient _httpClient;
        private readonly IUrlParser _urlParser;
        private readonly IDictionary<string, IParsedRedirect> _oldUrlRedirectIndex;
        private readonly IDictionary<string, HttpResponse> _responseCache;
        private readonly IList<IResult> _results;
        private readonly IList<string> _skipResultTypes;

        public RedirectProcessor(
            IConfiguration configuration,
            IUrlHelper urlHelper,
            IHttpClient httpClient,
            IUrlParser urlParser)
        {
            _configuration = configuration;
            _urlHelper = urlHelper;
            _httpClient = httpClient;
            _urlParser = urlParser;

            _oldUrlRedirectIndex = new Dictionary<string, IParsedRedirect>();
            _responseCache = new Dictionary<string, HttpResponse>();
            _results = new List<IResult>();
            _skipResultTypes = new List<string>
            {
                ResultTypes.InvalidResult,
                ResultTypes.IdenticalResult,
                ResultTypes.ExcludedRedirect
            };
        }

        public string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        public IEnumerable<IResult> Results
        {
            get
            {
                return _results;
            }
        }

        public void PreloadParsedRedirects(IEnumerable<IParsedRedirect> parsedRedirects)
        {
            var testHttpClient = _httpClient as TestHttpClient;

            foreach (var parsedRedirect in parsedRedirects
                .Where(r => r.IsValid && !_urlHelper.AreIdentical(
                    r.OldUrl, r.NewUrl))
                .ToList())
            {
                // add response for new url with configured status code,
                // if using test http client and status code is defined
                if (testHttpClient != null && 
                    _configuration.TestHttpClientNewUrlStatusCode.HasValue)
                {
                    var newUrlFormatted = 
                        _urlHelper.FormatUrl(
                            parsedRedirect.NewUrl.Parsed);
                    testHttpClient.Responses[newUrlFormatted] = new HttpResponse
                    {
                        StatusCode = (HttpStatusCode)_configuration.TestHttpClientNewUrlStatusCode
                    };
                }

                var oldUrlFormatted = 
                    _urlHelper.FormatUrl(
                        parsedRedirect.OldUrl.Parsed);

                if (_configuration.DuplicateOldUrlStrategy == DuplicateUrlStrategy.KeepFirst)
                {
                    if (_oldUrlRedirectIndex.ContainsKey(oldUrlFormatted))
                    {
                        continue;
                    }
                }

                _oldUrlRedirectIndex.Add(oldUrlFormatted, parsedRedirect);
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            // skip processed redirect if it's invalid or contains one or more of skip result types
            if (!processedRedirect.ParsedRedirect.IsValid ||
                processedRedirect.Results
                .Select(r => r.Type)
                .Distinct()
                .Count(r => _skipResultTypes.Contains(r, StringComparer.OrdinalIgnoreCase)) > 0)
            {
                return;
            }

            var checkRedirect = false;
            var isCyclicRedirect = false;
            var redirectCount = -1;
            var urlsVisited = new List<string>();
            var urlsIndex = new HashSet<string>();

            IUrl url = null;
            IUrl newUrl = processedRedirect.ParsedRedirect.OldUrl;
            UrlResponseResult urlResponseResult = null;

            do
            {
                url = newUrl;
                newUrl = null;

                //var parsedUrl = url.Parsed.AbsoluteUri;
                var urlFormatted = _urlHelper.FormatUrl(url.Parsed);

                redirectCount++;
                checkRedirect = false;

                // get response from url
                HttpResponse response;
                if (_responseCache.ContainsKey(urlFormatted))
                {
                    // get response from cache
                    response = _responseCache[urlFormatted];
                }
                else
                {
                    // get response from url
                    response = _httpClient.Get(urlFormatted);

                    // add response to cache
                    _responseCache.Add(urlFormatted, response);
                }

                // set has redirect and url to response location, 
                // if url returns 301 and has location
                if (response != null)
                {
                    var statusCode = response.StatusCode.HasValue
                        ? (int)response.StatusCode
                        : 0;
                    var locationUrl = !Regex.IsMatch(
                        response.Location ?? string.Empty, "https?://", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                        ? new Uri(url.Parsed, response.Location).AbsoluteUri
                        : response.Location ?? string.Empty;
                    urlResponseResult = new UrlResponseResult
                    {
                        Type = ResultTypes.UrlResponse,
                        Message = string.Format(
                            "Url '{0}' returned response with status code '{1}'",
                            url.Parsed.AbsoluteUri,
                            statusCode),
                        Url = url,
                        StatusCode = statusCode,
                        Location = locationUrl
                    };

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Moved:
                            // url returns 301
                            // update redirect with url from location
                            newUrl = _urlParser.ParseUrl(locationUrl);
                            break;
                        case HttpStatusCode.NotFound:
                            // url returns 404, check if a redirect exists
                            checkRedirect = true;
                            break;
                        default:
                            // urls not returning 301 or 404 are considered a url with a response
                            // stop redirecting
                            url = null;
                            break;
                    }
                }
                else
                {
                    urlResponseResult = null;
                }

                // check redirect for url
                if (checkRedirect && 
                    _oldUrlRedirectIndex.ContainsKey(urlFormatted))
                {
                    // update redirect with new url from existing redirect
                    newUrl = _oldUrlRedirectIndex[urlFormatted].NewUrl;
                }

                // cyclic redirect, if url and new url is not https redirect and url exists in url index
                if (newUrl != null && !_urlHelper.IsHttpsRedirect(
                    url,
                    newUrl) &&
                    urlsIndex.Contains(_urlHelper.FormatUrl(newUrl.Parsed)))
                {
                    isCyclicRedirect = true;
                    break;
                }

                // add formatted url to urls index, if it doesn't exist
                if (!urlsIndex.Contains(urlFormatted))
                {
                    urlsIndex.Add(urlFormatted);
                }

                // add formatted url to urls visited
                urlsVisited.Add(urlFormatted);
            } while (newUrl != null &&
                newUrl.Parsed != null &&
                redirectCount < 20);

            // add url response result, if it's defined
            if (urlResponseResult != null)
            {
                processedRedirect.Results.Add(
                    urlResponseResult);
                _results.Add(urlResponseResult);
            }

            if (isCyclicRedirect)
            {
                // add cyclic redirect result
                var cyclicResult = new RedirectResult
                {
                    Type = ResultTypes.CyclicRedirect,
                    Message =
                    string.Format(
                        "Cyclic redirect for urls '{0}'",
                        string.Join(",", urlsVisited)),
                    Url = url,
                    RedirectCount = redirectCount
                };
                processedRedirect.Results.Add(
                    cyclicResult);
                _results.Add(cyclicResult);
            }
            else if (redirectCount >= _configuration.MaxRedirectCount)
            {
                // add too many redirects result as redirect count is 
                // higher then max redirect count
                var tooManyRedirectsResult = new RedirectResult
                {
                    Type = ResultTypes.TooManyRedirects,
                    Message = string.Format(
                        "Too many redirect from url '{0}' exceeding max redirect count of {1}",
                        url,
                        _configuration.MaxRedirectCount),
                    Url = url,
                    RedirectCount = redirectCount
                };
                processedRedirect.Results.Add(
                    tooManyRedirectsResult);
                _results.Add(
                    tooManyRedirectsResult);
            }
            else if (redirectCount > 1 && redirectCount < _configuration.MaxRedirectCount)
            {
                // add optimized redirect result as redirect count is higher than 1
                // and less than max redirect count
                var optimizedRedirectResult = new RedirectResult
                {
                    Type = ResultTypes.OptimizedRedirect,
                    Message = string.Format(
                        "Optimized redirect to urls '{0}'",
                        string.Join(",", urlsVisited)),
                    Url = url,
                    RedirectCount = redirectCount
                };
                processedRedirect.Results.Add(
                    optimizedRedirectResult);
                _results.Add(
                    optimizedRedirectResult);
            }
        }
    }
}