using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class HttpResponse
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Content { get; set; }

        public HttpResponse()
        {
            Headers = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
        }
    }
}