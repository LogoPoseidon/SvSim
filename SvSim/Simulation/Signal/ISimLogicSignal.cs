using System.Numerics;

namespace SvSim.Simulation.Signal;

public interface ISimLogicSignal : ISimEventSource
{
    int BitWidth { get; }

    SimLogic<TOut> ReadAsLogic<TOut>() where TOut : IBinaryInteger<TOut>;
    
    SimLogic<BigInteger> ReadSlice(int msb, int lsb);
    void WriteSlice(int msb, int lsb, SimLogic<BigInteger> value);
    
    void AssignFromBigInteger(BigInteger value, BigInteger unknown = default);
}