using System;
using System.Net.Sockets;

namespace FirstRealize.App.WebRedirects.Core.Extensions
{
    public static class SocketExtensions
    {
        public static void Connect(
            this Socket socket,
            string host,
            int port,
            TimeSpan timeout)
        {
            var result = socket.BeginConnect(host, port, null, null);

            bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
            if (success)
            {
                socket.EndConnect(result);
            }
            else
            {
                socket.Close();
                throw new SocketException(10060); // Connection timed out.
            }
        }
    }
}