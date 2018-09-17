using FirstRealize.App.WebRedirects.Core.Readers;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class RedirectReaderTests
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [Test]
        public void CanReadRedirects()
        {
            var redirectReader = new RedirectReader(
                Path.Combine(AssemblyDirectory, "redirects.csv"));
            var redirects = redirectReader
                .ReadAllRedirects()
                .ToList();
            Assert.AreNotEqual(0, redirects.Count);
        }
    }
}