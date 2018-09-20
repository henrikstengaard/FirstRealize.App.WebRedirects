using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.ParserTests
{
    [TestFixture]
    public class ArgumentParserTests
    {
        [Test]
        public void ParseArgumentValue()
        {
            // test arguments
            var arguments = new[]
            {
                "--config",
                "\"configuration.json\""
            };

            // create argument parser
            var argumentParser = 
                new ArgumentParser(arguments);

            // parse argument value
            var argumentValue = 
                argumentParser.ParseArgumentValue(
                    "^(-c|--config)");

            // verify long argument value is parsed
            Assert.AreEqual(
                "\"configuration.json\"",
                argumentValue);
        }
    }
}