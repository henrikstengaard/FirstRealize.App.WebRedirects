using System.Net;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class HttpClient : IHttpClient
    {
        public HttpResponse Get(
            string url)
        {
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