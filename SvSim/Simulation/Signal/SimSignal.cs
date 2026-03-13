using System.Runtime.CompilerServices;

namespace SvSim.Simulation.Signal;

public abstract class SimSignal<TData>(
    string name,
    string vcdId,
    int width,
    TData initialValue,
    Action? onUpdate,
    Action<ITraceableSignal>? onDirty)
    : ITraceableSignal
    where TData : IEquatable<TData>
{
    public string HierarchicalName { get; } = name;
    public string VcdId { get; } = vcdId;
    public int BitWidth { get; } = width;

    public bool IsDirty { get; private set; }

    protected TData CurrentValue = initialValue;

    public TData Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => CurrentValue;
    }

    public void Assign(TData newValue)
    {
        if (CurrentValue.Equals(newValue)) return;

        CurrentValue = newValue;
    
        if (!IsDirty)
        {
            IsDirty = true;
            onDirty?.Invoke(this);
        }

        onUpdate?.Invoke();
    }

    public void ClearDirty() => IsDirty = false;

    public abstract string GetVcdValueString();

}