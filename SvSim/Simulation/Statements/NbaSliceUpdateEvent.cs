using System.Numerics;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class NbaSliceUpdateEvent(
    ISimLogicSignal signal, 
    int msb, 
    int lsb, 
    SimLogic<BigInteger> value) : ISimEvent
{
    public void Execute()
    {
        signal.WriteSlice(msb, lsb, value);
    }

    public void Trigger() { }
}