using FirstRealize.App.WebRedirects.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class HttpClient : IHttpClient
    {
        public readonly IDictionary<string, string> Headers;
        private readonly IdnMapping _idn;

        public string Protocol { get; set; }
        public TimeSpan Timeout { get; set; }

        public HttpClient()
        {
            Headers = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                { "Accept", "*/*" },
                { "User-Agent", "WebRedirects Crawler" }
            };
            _idn = new IdnMapping();
            Protocol = "HTTP/1.1";
            Timeout = TimeSpan.FromMinutes(1);
        }

        public HttpResponse Get(string url)
        {
            var urlMatch = Regex.Match(
                url,
                "^([a-z]+)://([^/]+):?([^/]*)(.*)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (!urlMatch.Success)
            {
                throw new HttpException(
                    string.Format(
                        "Invalid url '{0}'",
                        url));
            }

            var scheme = urlMatch.Groups[1].Value;
            var host =
                _idn.GetAscii(urlMatch.Groups[2].Value);
            var pathAndQuery = urlMatch.Groups[4].Value;

            int port;
            if (!string.IsNullOrWhiteSpace(urlMatch.Groups[3].Value) &&
                !int.TryParse(urlMatch.Groups[3].Value, out port))
            {
                throw new HttpException(
                    string.Format(
                        "Invalid port '{0}'",
                        urlMatch.Groups[3].Value));
            }
            else
            {
                port = scheme.Equals("https")
                    ? 443
                    : 80;
            }

            string response = SendRequest(
                host,
                port,
                BuildRequest(
                    "GET",
                    host,
                    pathAndQuery));
            return ParseResponse(
                response);
        }

        private string SendRequest(
            string host,
            int port,
            string request)
        {

            using (var client = new TcpClient(host, port))
            {
                using (var sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateRemoteCertificate),
                    null))
                {
                    try
                    {
                        sslStream.AuthenticateAsClient(host);
                    }
                    catch (AuthenticationException e)
                    {
                        throw new HttpException(
                            string.Format("Failed to connect: {0}", e.Message),
                            e);
                    }

                    sslStream.Write(Encoding.ASCII.GetBytes(request));
                    sslStream.Flush();

                    var data = new List<byte>();
                    var buffer = new byte[4096];
                    int result;
                    do
                    {
                        result = sslStream.Read(buffer, 0, buffer.Length);
                        data.AddRange(buffer);
                    } while (result == buffer.Length);

                    return Encoding.ASCII.GetString(
                        data.ToArray());
                }
            }

            //using (var socket = new Socket(
            //    AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            //{
            //    socket.Connect(host, port, Timeout);

            //    using (var stream = new SslStream(new NetworkStream(socket)))
            //    {
            //        stream.Write(Encoding.ASCII.GetBytes(request));
            //        stream.Flush();

            //        var data = new List<byte>();
            //        var buffer = new byte[4096];
            //        int result;
            //        do
            //        {
            //            result = socket.Receive(buffer);
            //            data.AddRange(buffer);

            //        } while (result == buffer.Length);

            //        return Encoding.ASCII.GetString(
            //            data.ToArray());
            //    }
            //}
        }

        private static bool ValidateRemoteCertificate(
          object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors policyErrors)
        {
            return true;
        }

        private HttpResponse ParseResponse(
            string response)
        {
            var responseLines = response
                .Replace("\r", "")
                .Split('\n');

            var statusMatch = Regex.Match(
                responseLines[0], "^(http[^\\s]+)\\s+([0-9]+)\\s+?(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (!statusMatch.Success)
            {
                throw new HttpException(
                    string.Format(
                        string.Format(
                            "Invalid header '{0}'",
                            responseLines[0])));
            }

            int statusCode;
            int.TryParse(statusMatch.Groups[2].Value, out statusCode);
            var statusDescription = statusMatch.Groups[3].Value;

            var headers = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            var headerIndex = 0;
            string responseLine;
            do
            {
                headerIndex++;
                responseLine = (responseLines[headerIndex] ?? string.Empty);

                if (string.IsNullOrWhiteSpace(responseLine))
                {
                    break;
                }

                var headerMatch = Regex.Match(
                    responseLine,
                    "^([^:]+):\\s+(.*)",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (!headerMatch.Success)
                {
                    throw new HttpException(
                        string.Format(
                            string.Format(
                                "Invalid header '{0}'",
                                responseLine)));
                }

                headers[headerMatch.Groups[1].Value] = headerMatch.Groups[2].Value;

            } while (headerIndex < responseLines.Length);

            var contentLines = new List<string>();

            for (var i = headerIndex + 1; i < responseLines.Length; i++)
            {
                contentLines.Add(responseLines[i]);
            }

            return new HttpResponse
            {
                StatusCode = statusCode,
                StatusDescription = statusDescription,
                Headers = headers,
                Content = string.Join(
                    "\r\n", contentLines)
            };
        }

        private string BuildRequest(
            string method,
            string host,
            string pathAndQuery)
        {
            var requestLines = new List<string>();

            requestLines.Add(
                string.Format("{0} {1} {2}",
                method.ToUpper(),
                FormatPathAndQuery(
                    pathAndQuery),
                Protocol));

            requestLines.Add(
                string.Format("Host: {0}",
                host));

            foreach (var header in Headers)
            {
                requestLines.Add(
                    string.Format("{0}: {1}",
                    header.Key,
                    header.Value));
            }

            requestLines.Add(string.Empty);

            return string.Join(
                "",
                requestLines.Select(
                    l => string.Concat(l, "\r\n")));
        }

        private string FormatPathAndQuery(string pathAndQuery)
        {
            return Regex.Replace(
                pathAndQuery,
                "%[a-z0-9]{2}",
                FormatEscapedData,
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private string FormatEscapedData(Match match)
        {
            return match.Value.ToLower();
        }
    }
}