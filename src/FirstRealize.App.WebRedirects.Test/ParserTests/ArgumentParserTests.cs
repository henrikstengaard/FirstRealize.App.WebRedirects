using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.ParserTests
{
    [TestFixture]
    public class ArgumentParserTests
    {
        [Test]
        public void ParseArgumentSwitch()
        {
            // arguments
            var arguments = new[]
            {
                "--process"
            };

            // create argument parser
            var argumentParser =
                new ArgumentParser(arguments);

            // parse argument switch
            var argumentSwitch =
                argumentParser.ParseArgumentSwitch(
                    "^(-p|--process)$");

            // verify argument switch is parsed
            Assert.AreEqual(
                true,
                argumentSwitch);
        }

        [Test]
        public void ParseArgumentValue()
        {
            // arguments
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
                    "^(-c|--config)$");

            // verify argument value is parsed
            Assert.AreEqual(
                "\"configuration.json\"",
                argumentValue);
        }
    }
}