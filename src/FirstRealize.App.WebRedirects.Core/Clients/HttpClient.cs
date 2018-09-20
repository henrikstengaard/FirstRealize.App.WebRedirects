using FirstRealize.App.WebRedirects.Core.Configuration;
using System.Net;
using System.Text.RegularExpressions;

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
            if (_configuration.ForceHttp)
            {
                url = Regex.Replace(
                    url,
                    "^https?://",
                    "http://",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "WebRedirects Crawler";
            request.AllowAutoRedirect = false;

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
                httpResponse.StatusCode = response.StatusCode;
            }

            return httpResponse;
        }
    }
}