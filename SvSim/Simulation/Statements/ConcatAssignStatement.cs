using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class ConcatAssignStatement(ISimLogicSignal[] lhsSignals, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = rhs.Evaluate().Value; 
        var unk = rhs.Evaluate().Unknown; 
        
        var currentShift = 0;
        for (var i = lhsSignals.Length - 1; i >= 0; i--)
        {
            var sig = lhsSignals[i];
            
            var mask = (BigInteger.One << sig.BitWidth) - BigInteger.One;
            
            var slicedVal = (val >> currentShift) & mask;
            var slicedUnk = (unk >> currentShift) & mask;
            
            sig.AssignFromBigInteger(slicedVal, slicedUnk);
            
            currentShift += sig.BitWidth;
        }
        yield break;
    }
}