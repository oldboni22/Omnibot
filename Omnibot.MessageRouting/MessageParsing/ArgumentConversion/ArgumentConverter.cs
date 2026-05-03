namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

public interface IArgumentConverter 
{
    object? ConvertUntyped(string? rawArgs);
    
    bool PropagateException { get; }
}

public abstract class ArgumentConverter<T> : IArgumentConverter
{
    public abstract T? Convert(string? rawArgs);

    public virtual bool PropagateException { get; } = true;
    
    object? IArgumentConverter.ConvertUntyped(string? rawArgs) => Convert(rawArgs);
}
