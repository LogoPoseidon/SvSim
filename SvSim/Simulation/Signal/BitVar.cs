using System.Numerics;

namespace SvSim.Simulation.Signal;

public sealed class BitVar<T> : SimVar<T> where T : IBinaryInteger<T>, IUnsignedNumber<T>
{
    private readonly T _mask;

    public BitVar(int width, T initialValue = default!) 
        : base(width, initialValue)
    {
        _mask = SvMath.GetMask<T>(width);
        Value = ApplyMask(initialValue);
    }

    protected override T ApplyMask(T value) => value & _mask;
}

public sealed class TracedBitVar<T> : TracedVar<T> where T : IBinaryInteger<T>, IUnsignedNumber<T>
{
    private readonly T _mask;

    public TracedBitVar(string name, string vcdId, int width, T initialValue = default!, Action<ITraceableSignal>? onDirty = null)
        : base(name, vcdId, width, initialValue, onDirty!)
    {
        _mask = SvMath.GetMask<T>(width);
        Value = ApplyMask(initialValue);
    }

    protected override T ApplyMask(T value) => value & _mask;

    public override string GetVcdValueString() =>
        BitWidth == 1 ? (Value == T.Zero ? "0" : "1") 
            : "b" + Convert.ToString(long.CreateTruncating(Value), 2).PadLeft(BitWidth, '0');
}