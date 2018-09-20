namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IArgumentParser
    {
        bool ParseArgumentSwitch(
            string argumentNamePattern);
        string ParseArgumentValue(
            string argumentNamePattern);
    }
}