using System.Security.Cryptography;
using System.Text;

namespace FirstRealize.App.WebRedirects.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToMd5(this string text)
        {
            return new MD5CryptoServiceProvider()
                .ComputeHash(Encoding.Default.GetBytes(text)).ToHexString();
        }
    }
}
