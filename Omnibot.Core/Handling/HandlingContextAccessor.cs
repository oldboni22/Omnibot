namespace Omnibot.Core.Handling;

public interface IHandlingContextAccessor
{
    HandlingContext? Context { get; internal set; }
}

internal sealed class HandlingContextAccessor : IHandlingContextAccessor
{
    private class HandlingContextHolder
    {
        public HandlingContext? Context;
    }
    
    private static readonly AsyncLocal<HandlingContextHolder> Storage = new();

    public HandlingContext? Context { get => Storage.Value?.Context; 
        set 
        {
            var holder = Storage.Value;
            holder?.Context = null;

            if (value is null)
            {
                Storage.Value = null!;
            }
            else
            {
                Storage.Value = new HandlingContextHolder { Context = value };
            }
        } 
    }
}
