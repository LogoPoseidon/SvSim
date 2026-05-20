using System.Numerics;
using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Signal;

public class PackedUnionVar(int width, Dictionary<string, int> memberWidths, SimLogic<BigInteger> initialValue)
    : IStructSignal
{
    private readonly LogicVar<BigInteger> _underlying = new(width, initialValue);

    public string StructTypeName { get; set; } = "";
    public int BitWidth => _underlying.BitWidth;
    public long EnumTypeId 
    { 
        get => _underlying.EnumTypeId; 
        set => _underlying.EnumTypeId = value; 
    }

    public SimLogic<BigInteger> ReadMember(string memberName)
    {
        return memberWidths.TryGetValue(memberName, out var memberWidth) 
            ? ReadSlice(memberWidth - 1, 0) 
            : throw new KeyNotFoundException($"Member '{memberName}' not found in packed union.");
    }

    public void WriteMember(string memberName, SimLogic<BigInteger> value)
    {
        if (!memberWidths.TryGetValue(memberName, out var memberWidth))
            throw new KeyNotFoundException($"Member '{memberName}' not found in packed union.");
        WriteSlice(memberWidth - 1, 0, value);
    }

    public (int Msb, int Lsb) GetMemberLayout(string memberName)
    {
        return memberWidths.TryGetValue(memberName, out var width) 
            ? (width - 1, 0) 
            : throw new KeyNotFoundException($"Member '{memberName}' not found in packed union.");
    }
    
    public SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut> => _underlying.ReadAsLogic<TOut>();
    public SimLogic<BigInteger> ReadSlice(int msb, int lsb) => _underlying.ReadSlice(msb, lsb);
    public void WriteSlice(int msb, int lsb, SimLogic<BigInteger> value) => _underlying.WriteSlice(msb, lsb, value);
    public void AssignFromBigInteger(BigInteger value, BigInteger unknown = default) => _underlying.AssignFromBigInteger(value, unknown);
    public BigInteger GetValueAsBigInt() => _underlying.GetValueAsBigInt();

    public void Subscribe(ISimEvent consumer) => _underlying.Subscribe(consumer);
    public void Unsubscribe(ISimEvent consumer) => _underlying.Unsubscribe(consumer);
}