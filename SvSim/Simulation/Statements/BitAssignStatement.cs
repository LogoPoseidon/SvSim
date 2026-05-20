using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class BitAssignStatement(ISimLogicSignal signal, IExpression<SimLogic<BigInteger>> indexExpr, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var idx = (int)indexExpr.Evaluate().Value;
        var value = rhs.Evaluate();
        signal.WriteSlice(idx, idx, value);
        yield break;
    }
}