using FirstRealize.App.WebRedirects.Core.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private readonly IUrlParser _urlParser;
        public readonly IDictionary<string, string> Headers;
        private readonly IdnMapping _idn;

        public string Protocol { get; set; }
        public TimeSpan Timeout { get; set; }

        public HttpClient(
            IUrlParser urlParser)
        {
            _urlParser = urlParser;
            Headers = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                { "User-Agent", "WebRedirects/1.0" },
                { "Accept", "*/*" },
                { "Connection", "keep-alive" },
                { "Keep-Alive", "300" },
                { "Pragma", "no-cache" },
                { "Cache", "no-cache" }
            };
            _idn = new IdnMapping();
            Protocol = "HTTP/1.1";
            Timeout = TimeSpan.FromMinutes(1);
        }

        public HttpResponse Get(string url)
        {
            var parsedUrl = _urlParser.Parse(
                url,
                null);

            if (!parsedUrl.IsValid)
            {
                throw new HttpException(
                    string.Format(
                        "Invalid url '{0}'",
                        url));
            }

            var host =
                _idn.GetAscii(parsedUrl.Host);

            var response = SendRequest(
                host,
                parsedUrl.Port,
                parsedUrl.Scheme.ToLower().StartsWith("https"),
                BuildRequest(
                    "GET",
                    host,
                    parsedUrl.PathAndQuery));
            return ParseData(
                response);
        }

        private byte[] SendRequest(
            string host,
            int port,
            bool ssl,
            string request)
        {
            var requestBytes = 
                Encoding.ASCII.GetBytes(request);

            using (var client = new TcpClient(host, port))
            {
                if (ssl)
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

                        sslStream.Write(
                            requestBytes);
                        sslStream.Flush();

                        return ReadData(sslStream);
                    }
                }
                else
                {
                    using (var stream = client.GetStream())
                    {
                        stream.Write(
                            requestBytes,
                            0,
                            requestBytes.Length);
                        stream.Flush();

                        return ReadData(stream);
                    }
                }
            }
        }

        private static bool ValidateRemoteCertificate(
          object sender,
          X509Certificate certificate,
          X509Chain chain,
          SslPolicyErrors policyErrors)
        {
            return true;
        }

        private byte[] ReadData(Stream stream)
        {
            var data = new List<byte>();
            var buffer = new byte[4096];
            int result;
            do
            {
                result = stream.Read(buffer, 0, buffer.Length);
                data.AddRange(buffer);
            } while (result == buffer.Length);

            return data.ToArray();
        }

        private IEnumerable<string> ParseHeaderLines(
            byte[] data)
        {
            var headerLineStart = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if (i > 0 &&
                    data[i - 1] == 13 &&
                    data[i] == 10)
                {
                    var headerLineBytes = new byte[i - headerLineStart - 1];
                    Array.Copy(data, headerLineStart, headerLineBytes, 0, headerLineBytes.Length);
                    headerLineStart = i + 1;
                    var headerLine =
                        Encoding.ASCII.GetString(headerLineBytes);
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        break;
                    }
                    yield return headerLine;
                }
            }
        }

        private HttpResponse ParseData(
            byte[] data)
        {
            var headerLines = ParseHeaderLines(data)
                .ToList();

            var statusMatch = Regex.Match(
                headerLines[0], "^(http[^\\s]+)\\s+([0-9]+)\\s+?(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (!statusMatch.Success)
            {
                throw new HttpException(
                    string.Format(
                        string.Format(
                            "Invalid header '{0}'",
                            headerLines[0])));
            }

            int statusCode;
            int.TryParse(statusMatch.Groups[2].Value, out statusCode);
            var statusDescription = statusMatch.Groups[3].Value;

            var headers = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            var headerIndex = 1;
            string headerLine;
            while(headerIndex < headerLines.Count)
            {
                headerLine = (headerLines[headerIndex] ?? string.Empty);

                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    break;
                }

                var headerMatch = Regex.Match(
                    headerLine,
                    "^([^:]+):\\s+(.*)",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (!headerMatch.Success)
                {
                    throw new HttpException(
                        string.Format(
                            string.Format(
                                "Invalid header '{0}'",
                                headerLine)));
                }

                headers[headerMatch.Groups[1].Value] = headerMatch.Groups[2].Value;
                headerIndex++;
            }

            return new HttpResponse
            {
                StatusCode = statusCode,
                StatusDescription = statusDescription,
                Headers = headers
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