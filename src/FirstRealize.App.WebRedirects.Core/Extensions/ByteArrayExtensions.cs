using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            return string.Join("", bytes.Select(x => x.ToString("x2")));
        }
    }
}