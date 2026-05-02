using Omnibot.Core.Handling;

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

public interface IConversionFailHandler
{
    Task<bool> Handle(HandlingContext context);
}
