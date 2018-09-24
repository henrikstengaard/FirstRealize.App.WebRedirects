﻿using System;
using System.Collections.Generic;
using System.Net;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class TestHttpClient : IHttpClient
    {
        public TestHttpClient()
        {
            Responses = new Dictionary<string,HttpResponse>(
                StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, HttpResponse> Responses { get; }

        public HttpResponse Get(
            string url)
        {
            if (!Responses.ContainsKey(url))
            {
                return new HttpResponse
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            return Responses[url];
        }
    }
}