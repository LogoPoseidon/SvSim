using System.Runtime.CompilerServices;
using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Signal;

public abstract class SimVar<TData>(int width, TData initialValue) : ISimEventSource
    where TData : IEquatable<TData>
{
    public int BitWidth { get; } = width;
    public TData Value { get; protected set; } = initialValue;
    
    private readonly List<ISimEvent> _subscribers = [];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract TData ApplyMask(TData value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void NotifySubscribers()
    {
        foreach (var t in _subscribers)
        {
            t.Trigger();
        }
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void Assign(TData newValue)
    {
        var maskedValue = ApplyMask(newValue);
        if (Value.Equals(maskedValue)) return;
    
        Value = maskedValue;
        NotifySubscribers();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe(ISimEvent consumer)
    {
        if (!_subscribers.Contains(consumer))
            _subscribers.Add(consumer);
    }
}