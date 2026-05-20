using System.Numerics;
using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Signal;

public class PackedStructVar(
    int width,
    Dictionary<string, (int Msb, int Lsb)> layout,
    SimLogic<BigInteger> initialValue)
    : IStructSignal 
{
    public string StructTypeName { get; set; } = "";
    private readonly LogicVar<BigInteger> _underlying = new(width, initialValue);

    public int BitWidth => _underlying.BitWidth;
    public long EnumTypeId 
    { 
        get => _underlying.EnumTypeId; 
        set => _underlying.EnumTypeId = value; 
    }

    public SimLogic<BigInteger> ReadMember(string memberName)
    {
        return layout.TryGetValue(memberName, out var slice) 
            ? ReadSlice(slice.Msb, slice.Lsb) 
            : throw new KeyNotFoundException($"Member '{memberName}' not found in packed struct.");
    }

    public void WriteMember(string memberName, SimLogic<BigInteger> value)
    {
        if (!layout.TryGetValue(memberName, out var slice))
            throw new KeyNotFoundException($"Member '{memberName}' not found in packed struct.");
        WriteSlice(slice.Msb, slice.Lsb, value);
    }
    
    public (int Msb, int Lsb) GetMemberLayout(string memberName)
    {
        return layout.TryGetValue(memberName, out var slice) 
            ? slice 
            : throw new KeyNotFoundException($"Member '{memberName}' not found in packed struct.");
    }

    public SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut> => _underlying.ReadAsLogic<TOut>();
    public SimLogic<BigInteger> ReadSlice(int msb, int lsb) => _underlying.ReadSlice(msb, lsb);
    public void WriteSlice(int msb, int lsb, SimLogic<BigInteger> value) => _underlying.WriteSlice(msb, lsb, value);
    public void AssignFromBigInteger(BigInteger value, BigInteger unknown = default) => _underlying.AssignFromBigInteger(value, unknown);
    public BigInteger GetValueAsBigInt() => _underlying.GetValueAsBigInt();

    public void Subscribe(ISimEvent consumer) => _underlying.Subscribe(consumer);
    public void Unsubscribe(ISimEvent consumer) => _underlying.Unsubscribe(consumer);
}