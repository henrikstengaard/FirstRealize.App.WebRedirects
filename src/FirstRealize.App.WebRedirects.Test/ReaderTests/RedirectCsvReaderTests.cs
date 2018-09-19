using FirstRealize.App.WebRedirects.Core.Readers;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class RedirectCsvReaderTests
    {
        [Test]
        public void CanReadRedirects()
        {
            var redirectReader = new RedirectCsvReader(
                Path.Combine(TestData.TestData.CurrentDirectory, @"TestData\redirects.csv"));
            var redirects = redirectReader
                .ReadAllRedirects()
                .ToList();
            Assert.AreNotEqual(0, redirects.Count);
        }
    }
}