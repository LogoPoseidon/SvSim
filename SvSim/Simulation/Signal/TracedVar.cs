using System.Runtime.CompilerServices;

namespace SvSim.Simulation.Signal;

public abstract class TracedVar<TData>(
    string name,
    string vcdId,
    int width,
    TData initialValue,
    Action<ITraceableSignal> onDirty)
    : SimVar<TData>(width, initialValue), ITraceableSignal
    where TData : IEquatable<TData>
{
    public string HierarchicalName { get; } = name;
    public string VcdId { get; } = vcdId;
    public bool IsDirty { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Assign(TData newValue)
    {
        var maskedValue = ApplyMask(newValue);
        
        if (Value.Equals(maskedValue)) return;
        
        Value = maskedValue;
        
        if (!IsDirty)
        {
            IsDirty = true;
            onDirty.Invoke(this); 
        }
        NotifySubscribers();
    }

    public void ClearDirty() => IsDirty = false;
    
    public abstract string GetVcdValueString();
}