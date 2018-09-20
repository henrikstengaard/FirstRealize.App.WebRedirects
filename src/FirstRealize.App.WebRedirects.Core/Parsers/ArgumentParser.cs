using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public class ArgumentParser : IArgumentParser
    {
        private readonly IList<string> _arguments;

        public ArgumentParser(
            IEnumerable<string> arguments)
        {
            _arguments = arguments.ToList();
        }

        public string ParseArgumentValue(
            string argumentNamePattern)
        {
            for (int i = 0; i < _arguments.Count; i++)
            {
                if (Regex.IsMatch(_arguments[i], argumentNamePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
                    && i + 1 < _arguments.Count)
                {
                    return _arguments[i + 1].Trim();
                }
            }

            return string.Empty;
        }
    }
}