using System.Net;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class HttpResponse
    {
        public HttpStatusCode? StatusCode { get; set; }
        public string Location { get; set; }
    }
}