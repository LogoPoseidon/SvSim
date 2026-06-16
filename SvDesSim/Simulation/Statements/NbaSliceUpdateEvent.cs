using System.Numerics;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

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