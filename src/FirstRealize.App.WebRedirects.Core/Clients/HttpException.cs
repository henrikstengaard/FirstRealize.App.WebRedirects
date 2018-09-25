using System;

namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public class HttpException : Exception
    {
        public HttpException(string message)
            : base(message)
        {
        }

        public HttpException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}