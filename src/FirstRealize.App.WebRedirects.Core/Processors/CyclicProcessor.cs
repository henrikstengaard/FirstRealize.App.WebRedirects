﻿using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    // rename to redirect processor
    public class CyclicProcessor : IProcessor, IProcessorPreload
    {
        private readonly IDictionary<string, Redirect> _oldUrlIndex;
        private readonly IConfiguration _configuration;
        private readonly IHttpClient _httpClient;
        private readonly IUrlParser _urlParser;

        public IDictionary<string, Url> OldUrlsWithOkStatusCode { get; }

        public CyclicProcessor(
            IConfiguration configuration,
            IHttpClient httpClient,
            IUrlParser urlParser)
        {
            _oldUrlIndex = new Dictionary<string, Redirect>();
            _configuration = configuration;
            _httpClient = httpClient;
            _urlParser = urlParser;
            OldUrlsWithOkStatusCode = new Dictionary<string, Url>();
        }

        private string FormatUrl(string url)
        {
            return _configuration.ForceHttp
                ? Regex.Replace(url, "^https?://", "http://", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                : url;
        }

        public void PreloadRedirects(IEnumerable<Redirect> redirects)
        {
            foreach(var redirect in redirects
                .Where(r => r.IsValid && !r.IsIdentical)
                .ToList())
            {
                var oldUrl = FormatUrl(
                    redirect.OldUrl.Parsed.AbsoluteUri);

                if (_oldUrlIndex.ContainsKey(oldUrl))
                {
                    continue;
                }

                _oldUrlIndex.Add(oldUrl, redirect);
            }
        }

        public void Process(IProcessedRedirect processedRedirect)
        {
            var checkRedirect = false;
            var isCyclicRedirect = false;
            var redirectCount = 0;
            var urlsVisited = new List<string>();
            var urlsIndex = new HashSet<string>();

            urlsVisited.Add(FormatUrl(
                processedRedirect.Redirect.OldUrl.Parsed.AbsoluteUri));

            Redirect redirect = processedRedirect.Redirect;

            do
            {
                var url = redirect.NewUrl.Parsed.AbsoluteUri;
                redirectCount++;
                checkRedirect = false;

                urlsVisited.Add(url);

                if (urlsIndex.Contains(url))
                {
                    isCyclicRedirect = true;
                    break;
                }

                urlsIndex.Add(url);

                // get url
                var response = _httpClient.Get(url);

                // set has redirect and url to response location, 
                // if url returns 301 and has location
                if (response != null)
                {
                    switch(response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            OldUrlsWithOkStatusCode[
                                redirect.NewUrl.Parsed.AbsoluteUri] = redirect.NewUrl;
                            redirect = null;
                            break;
                        case HttpStatusCode.Moved:
                            var newUrl = !Regex.IsMatch(
                                response.Location ?? string.Empty, "https?://", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                                ? new Uri(new Uri(url), response.Location).AbsoluteUri
                                : response.Location ?? string.Empty;
                            redirect = new Redirect
                            {
                                OldUrl = redirect.NewUrl,
                                NewUrl = new Url
                                {
                                    Raw = newUrl,
                                    Parsed = _urlParser.ParseUrl(newUrl)
                                }
                            };
                            break;
                        case HttpStatusCode.NotFound:
                            checkRedirect = true;
                            break;
                        default:
                            redirect = null;
                            break;
                    }
                }

                // if url didn't redirect, check if url exists in old url
                if (checkRedirect)
                {
                    if (_oldUrlIndex.ContainsKey(url))
                    {
                        var newUrl = FormatUrl(
                            _oldUrlIndex[url].NewUrl.Parsed.AbsoluteUri);
                        redirect = new Redirect
                        {
                            OldUrl = redirect.NewUrl,
                            NewUrl = _oldUrlIndex[url].NewUrl
                        };
                    }
                    else
                    {
                        redirect = null;
                    }
                }
            } while (redirect != null && 
                redirect.NewUrl.Parsed != null &&
                redirectCount < 20);

            if (redirectCount > 1)

            {
                //processedRedirect.Results.Add(
                //    new Result
                //    {
                //        Type = "Optimized",
                //        Message =
                //    string.Format(
                //        "Last redirect to url '{0}'",
                //        url)
                //    });
            }

            if (isCyclicRedirect)
            {
                processedRedirect.Results.Add(
                    new Result
                    {
                        Type = ResultTypes.Cyclic,
                        Message =
                    string.Format(
                        "Cyclic redirect for urls '{0}'",
                        string.Join(",", urlsVisited))
                    });
            }
        }
    }
}