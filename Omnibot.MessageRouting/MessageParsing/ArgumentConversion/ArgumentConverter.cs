namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

public interface IArgumentConverter 
{
    object? ConvertUntyped(string? rawArgs);
}

public abstract class ArgumentConverter<T> : IArgumentConverter
{
    public abstract T? Convert(string? rawArgs);
    
    object? IArgumentConverter.ConvertUntyped(string? rawArgs) => Convert(rawArgs);
}
