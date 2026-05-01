namespace Omnibot.MessageRouting.MessageParsing.CommandExtraction;

public sealed class CommandExtractionOptions
{
    internal const string Section = "Omnibot.MessageParser";

    public char StartSymbol { get; set; } = '/';
}
