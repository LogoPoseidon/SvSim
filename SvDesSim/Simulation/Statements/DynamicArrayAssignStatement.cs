using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

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