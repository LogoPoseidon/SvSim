using System.Numerics;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class NbaGeneralUpdateEvent(ISimLogicSignal lhs, SimLogic<BigInteger> val) : ISimEvent
{
    public void Execute() => lhs.AssignFromBigInteger(val.Value, val.Unknown);

    public void Trigger()
    {
    }
}