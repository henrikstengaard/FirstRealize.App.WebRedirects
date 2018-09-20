namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IArgumentParser
    {
        string ParseArgumentValue(
            string argumentNamePattern);
    }
}