using System.Numerics;

namespace SvSim.Simulation.Signal;

public sealed class WireNet<T> : SimVar<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    private readonly SimLogic<T> _maskLogic;

    public WireNet(int width) 
        : base(width, new SimLogic<T>(T.Zero, T.AllBitsSet))
    {
        var rawMask = SvMath.GetMask<T>(width);
        _maskLogic = new SimLogic<T>(rawMask, T.Zero);
        
        Value = ApplyMask(Value); 
    }

    protected override SimLogic<T> ApplyMask(SimLogic<T> value) => value & _maskLogic;
}

public sealed class TracedWireNet<T> : TracedVar<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    private readonly SimLogic<T> _maskLogic;

    public TracedWireNet(
            string name, 
            string vcdId, 
            int width, 
            Action<ITraceableSignal>? onDirty = null)
        : base(name, vcdId, width, new SimLogic<T>(T.Zero, T.AllBitsSet), onDirty!)
    {
        var rawMask = SvMath.GetMask<T>(width);
        _maskLogic = new SimLogic<T>(rawMask, T.Zero);
        
        Value = ApplyMask(Value); 
    }

    protected override SimLogic<T> ApplyMask(SimLogic<T> value) => value & _maskLogic;

    public override string GetVcdValueString() => Value.ToVcdString(BitWidth);
}