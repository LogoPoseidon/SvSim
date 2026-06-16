using System.Numerics;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NbaGeneralUpdateEvent(ISimLogicSignal lhs, SimLogic<BigInteger> val) : ISimEvent
{
    public void Execute() => lhs.AssignFromBigInteger(val.Value, val.Unknown);

    public void Trigger()
    {
    }
}