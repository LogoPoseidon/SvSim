using System.Numerics;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NbaConcatUpdateEvent(ISimLogicSignal[] lhsSignals, SimLogic<BigInteger> result) : ISimEvent
{
    public void Execute()
    {
        var currentShift = 0;
        for (var i = lhsSignals.Length - 1; i >= 0; i--)
        {
            var sig = lhsSignals[i];
            var mask = (BigInteger.One << sig.BitWidth) - BigInteger.One;
            
            sig.AssignFromBigInteger(
                (result.Value >> currentShift) & mask, 
                (result.Unknown >> currentShift) & mask
            );
            
            currentShift += sig.BitWidth;
        }
    }

    public void Trigger() {  }
}