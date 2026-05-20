using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class DynamicArrayAssignStatement(
    DynamicArrayVar<ISimLogicSignal> dyn,
    IExpression<SimLogic<BigInteger>> indexExpr,
    IExpression<SimLogic<BigInteger>> rhsExpr) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var index = (int)indexExpr.Evaluate().Value;
        var val = rhsExpr.Evaluate();
        dyn[index].AssignFromBigInteger(val.Value, val.Unknown);
        yield break;
    }
}