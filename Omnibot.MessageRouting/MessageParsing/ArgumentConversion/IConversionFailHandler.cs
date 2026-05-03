#region

using Omnibot.Core.Handling;

#endregion

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

public interface IConversionFailHandler
{
    Task<bool> Handle(HandlingContext context);
}
