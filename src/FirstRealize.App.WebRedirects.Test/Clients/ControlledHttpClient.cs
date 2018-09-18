using FirstRealize.App.WebRedirects.Core.Clients;
using System;
using System.Collections.Generic;
using System.Net;

namespace FirstRealize.App.WebRedirects.Test.Clients
{
    class ControlledHttpClient : IHttpClient
    {
        public ControlledHttpClient()
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