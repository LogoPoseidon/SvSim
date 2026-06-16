using System.Numerics;
using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public class SliceSignal(ISimLogicSignal parent, int msb, int lsb) : IStructSignal
{
    public int BitWidth => msb - lsb + 1;
    public string StructTypeName { get; set; } = "";
    public long EnumTypeId { get => parent.EnumTypeId; set => parent.EnumTypeId = value; }

    public SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut>
    {
        var slice = parent.ReadSlice(msb, lsb);
        return new SimLogic<TOut>(TOut.CreateTruncating(slice.Value), TOut.CreateTruncating(slice.Unknown));
    }

    public SimLogic<BigInteger> ReadSlice(int m, int l) => parent.ReadSlice(lsb + m, lsb + l);
    public void WriteSlice(int m, int l, SimLogic<BigInteger> value) => parent.WriteSlice(lsb + m, lsb + l, value);
    public void AssignFromBigInteger(BigInteger value, BigInteger unknown = default) => parent.WriteSlice(msb, lsb, new SimLogic<BigInteger>(value, unknown));
    public BigInteger GetValueAsBigInt() => parent.ReadSlice(msb, lsb).Value;
    
    public void Subscribe(ISimEvent consumer) => parent.Subscribe(consumer);
    public void Unsubscribe(ISimEvent consumer) => parent.Unsubscribe(consumer);
}