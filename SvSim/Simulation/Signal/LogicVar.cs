using System.Numerics;

namespace SvSim.Simulation.Signal;

public sealed class LogicVar<T> : SimVar<SimLogic<T>>, ISimLogicSignal where T : IBinaryInteger<T>
{
    private readonly SimLogic<T> _maskLogic;

    public LogicVar(int width, SimLogic<T> initialValue) 
        : base(width, initialValue)
    {
        var rawMask = SvMath.GetMask<T>(width);
        _maskLogic = new SimLogic<T>(rawMask, T.Zero); 
        Value = ApplyMask(initialValue);
    }
    
    public SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut>
    {
        var v = TOut.CreateTruncating(Value.Value);
        var u = TOut.CreateTruncating(Value.Unknown);
        return new SimLogic<TOut>(v, u);
    }

    public SimLogic<BigInteger> ReadSlice(int msb, int lsb)
    {
        var sliceWidth = msb - lsb + 1;
        var v = BigInteger.CreateTruncating(Value.Value);
        var u = BigInteger.CreateTruncating(Value.Unknown);

        var mask = (BigInteger.One << sliceWidth) - 1;
        var slicedV = (v >> lsb) & mask;
        var slicedU = (u >> lsb) & mask;

        return new SimLogic<BigInteger>(slicedV, slicedU);
    }

    public void WriteSlice(int msb, int lsb, SimLogic<BigInteger> value)
    {
        var sliceWidth = msb - lsb + 1;
        var mask = ((BigInteger.One << sliceWidth) - 1) << lsb;

        var currentV = BigInteger.CreateTruncating(Value.Value);
        var currentU = BigInteger.CreateTruncating(Value.Unknown);

        var newBitsV = (value.Value & ((BigInteger.One << sliceWidth) - 1)) << lsb;
        var newBitsU = (value.Unknown & ((BigInteger.One << sliceWidth) - 1)) << lsb;

        var finalV = (currentV & ~mask) | newBitsV;
        var finalU = (currentU & ~mask) | newBitsU;

        AssignFromBigInteger(finalV, finalU);
    }

    public void AssignFromBigInteger(BigInteger value, BigInteger unknown = default)
    {
        var v = T.CreateTruncating(value);
        var u = T.CreateTruncating(unknown);
        Assign(new SimLogic<T>(v, u));
    }

    protected override SimLogic<T> ApplyMask(SimLogic<T> value) => value & _maskLogic;
}

public sealed class TracedLogicVar<T> : TracedVar<SimLogic<T>>, ISimLogicSignal where T : IBinaryInteger<T>
{
    private readonly SimLogic<T> _maskLogic;

    public TracedLogicVar(string name, string vcdId, int width, SimLogic<T> initialValue, Action<ITraceableSignal>? onDirty = null)
        : base(name, vcdId, width, initialValue, onDirty!)
    {
        var rawMask = SvMath.GetMask<T>(width);
        _maskLogic = new SimLogic<T>(rawMask, T.Zero);
        Value = ApplyMask(initialValue);
    }
    
    public SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut>
    {
        var v = TOut.CreateTruncating(Value.Value);
        var u = TOut.CreateTruncating(Value.Unknown);
        return new SimLogic<TOut>(v, u);
    }

    public SimLogic<BigInteger> ReadSlice(int msb, int lsb)
    {
        var sliceWidth = msb - lsb + 1;
        var v = BigInteger.CreateTruncating(Value.Value);
        var u = BigInteger.CreateTruncating(Value.Unknown);

        var mask = (BigInteger.One << sliceWidth) - 1;
        var slicedV = (v >> lsb) & mask;
        var slicedU = (u >> lsb) & mask;

        return new SimLogic<BigInteger>(slicedV, slicedU);
    }

    public void WriteSlice(int msb, int lsb, SimLogic<BigInteger> value)
    {
        var sliceWidth = msb - lsb + 1;
        var mask = ((BigInteger.One << sliceWidth) - 1) << lsb;

        var currentV = BigInteger.CreateTruncating(Value.Value);
        var currentU = BigInteger.CreateTruncating(Value.Unknown);

        var newBitsV = (value.Value & ((BigInteger.One << sliceWidth) - 1)) << lsb;
        var newBitsU = (value.Unknown & ((BigInteger.One << sliceWidth) - 1)) << lsb;

        var finalV = (currentV & ~mask) | newBitsV;
        var finalU = (currentU & ~mask) | newBitsU;

        AssignFromBigInteger(finalV, finalU);
    }

    public void AssignFromBigInteger(BigInteger value, BigInteger unknown = default)
    {
        var v = T.CreateTruncating(value);
        var u = T.CreateTruncating(unknown);
        Assign(new SimLogic<T>(v, u));
    }

    protected override SimLogic<T> ApplyMask(SimLogic<T> value) => value & _maskLogic;

    public override string GetVcdValueString() => Value.ToVcdString(BitWidth);
}