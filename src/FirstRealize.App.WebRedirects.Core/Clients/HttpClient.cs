using FirstRealize.App.WebRedirects.Core.Configuration;
using System.Net;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class HttpClient : IHttpClient
    {
        private readonly IConfiguration _configuration;

        public HttpClient(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public HttpResponse Get(
            string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";

            var httpWebRequest = request as HttpWebRequest;
            if (httpWebRequest != null)
            {
                httpWebRequest.Accept = "*/*";
                httpWebRequest.UserAgent = "WebRedirects Crawler";
                httpWebRequest.AllowAutoRedirect = false;
            }

            var httpResponse = new HttpResponse();

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                httpResponse.StatusCode = response.StatusCode;
                httpResponse.Location = response.Headers["Location"];
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
                if (response != null)
                {
                    httpResponse.StatusCode = response.StatusCode;
                }
            }

            return httpResponse;
        }
    }
}