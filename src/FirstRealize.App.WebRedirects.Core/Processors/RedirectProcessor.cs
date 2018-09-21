using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
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
        private readonly IHttpClient _httpClient;
        private readonly IUrlParser _urlParser;
        private readonly IDictionary<string, IParsedRedirect> _oldUrlIndex;
        private readonly IDictionary<string, HttpResponse> _responseCache;
        private readonly IList<IResult> _results;

        public RedirectProcessor(
            IConfiguration configuration,
            IHttpClient httpClient,
            IUrlParser urlParser)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _urlParser = urlParser;

            _oldUrlIndex = new Dictionary<string, IParsedRedirect>();
            _responseCache = new Dictionary<string, HttpResponse>();
            _results = new List<IResult>();
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

        private string FormatUrl(string url)
        {
            return _configuration.ForceHttp
                ? Regex.Replace(url, "^https?://", "http://", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                : url;
        }

        public void PreloadParsedRedirects(IEnumerable<IParsedRedirect> parsedRedirects)
        {
            foreach (var parsedRedirect in parsedRedirects
                .Where(r => r.IsValid && !r.IsIdentical)
                .ToList())
            {
                var oldUrl = FormatUrl(
                    parsedRedirect.OldUrl.Parsed.AbsoluteUri);

                if (_oldUrlIndex.ContainsKey(oldUrl))
                {
                    continue;
                }

                _oldUrlIndex.Add(oldUrl, parsedRedirect);
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            if (!processedRedirect.ParsedRedirect.IsValid ||
                processedRedirect.ParsedRedirect.IsIdentical)
            {
                return;
            }

            var checkRedirect = false;
            var isCyclicRedirect = false;
            var redirectCount = 0;
            var urlsVisited = new List<string>();
            var urlsIndex = new HashSet<string>();

            var oldUrl = FormatUrl(
                processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri);
            urlsVisited.Add(oldUrl);
            urlsIndex.Add(oldUrl);

            var redirect = processedRedirect.ParsedRedirect;
            IUrl url = null;
            UrlResponseResult urlResponseResult = null;

            do
            {
                url = redirect.NewUrl;
                var parsedUrl = url.Parsed.AbsoluteUri;

                redirectCount++;
                checkRedirect = false;

                urlsVisited.Add(parsedUrl);

                var formattedUrl = FormatUrl(parsedUrl);
                if (urlsIndex.Contains(formattedUrl))
                {
                    isCyclicRedirect = true;
                    break;
                }
                urlsIndex.Add(formattedUrl);

                // get response from url
                HttpResponse response;
                if (_responseCache.ContainsKey(parsedUrl))
                {
                    // get response from cache
                    response = _responseCache[parsedUrl];
                }
                else
                {
                    // get response from url
                    response = _httpClient.Get(parsedUrl);

                    // add response to cache
                    _responseCache.Add(parsedUrl, response);
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
                            redirect.NewUrl.Parsed.AbsoluteUri,
                            statusCode),
                        Url = redirect.NewUrl,
                        StatusCode = statusCode,
                        Location = locationUrl
                    };

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Moved:
                            // url returns 301
                            // update redirect with url from location
                            redirect = new ParsedRedirect
                            {
                                OldUrl = redirect.NewUrl,
                                NewUrl = _urlParser.ParseUrl(locationUrl)
                            };
                            break;
                        case HttpStatusCode.NotFound:
                            // url returns 404, check if a redirect exists
                            checkRedirect = true;
                            break;
                        default:
                            // urls not returning 301 or 404 are considered a url with a response
                            // stop redirecting
                            //processedRedirect.Results.Add(
                            //    urlResponseResult);
                            //_results.Add(urlResponseResult);
                            redirect = null;
                            break;
                    }
                }
                else
                {
                    urlResponseResult = null;
                }

                // check redirect for url
                if (checkRedirect)
                {
                    if (_oldUrlIndex.ContainsKey(parsedUrl))
                    {
                        // update redirect with new url from existing redirect
                        var newUrl = FormatUrl(
                            _oldUrlIndex[parsedUrl].NewUrl.Parsed.AbsoluteUri);
                        redirect = new ParsedRedirect
                        {
                            OldUrl = redirect.NewUrl,
                            NewUrl = _oldUrlIndex[parsedUrl].NewUrl
                        };
                    }
                    else
                    {
                        // checked url doesn't have a redirect
                        // stop redirecting
                        redirect = null;
                    }
                }
            } while (redirect != null &&
                redirect.NewUrl.Parsed != null &&
                redirectCount < 20);

            // add url response result, if it's defined
            if (urlResponseResult != null)
            {
                processedRedirect.Results.Add(
                    urlResponseResult);
                _results.Add(urlResponseResult);
            }

            // add optimized redirect result, if redirect count is higher than 1 and less than max redirect count
            if (redirectCount > 1 && redirectCount < _configuration.MaxRedirectCount)
            {
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
            else if (redirectCount >= _configuration.MaxRedirectCount)
            {
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

            if (isCyclicRedirect)
            {
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
        }
    }
}