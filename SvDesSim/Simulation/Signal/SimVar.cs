using System.Runtime.CompilerServices;
using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public abstract class SimVar<TData>(int width, TData initialValue) : ISimEventSource
    where TData : IEquatable<TData>
{
    public int BitWidth { get; } = width;
    public TData Value { get; protected set; } = initialValue;
    
    private readonly HashSet<ISimEvent> _subscribers = [];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract TData ApplyMask(TData value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void NotifySubscribers()
    {
        var snapshot = _subscribers.ToArray();
        foreach (var t in snapshot)
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
        _subscribers.Add(consumer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe(ISimEvent consumer)
    {
        _subscribers.Remove(consumer);
    }
}