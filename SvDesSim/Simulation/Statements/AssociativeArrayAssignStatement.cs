using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class AssociativeArrayAssignStatement(
    AssociativeArrayVar<BigInteger, ISimLogicSignal> aa,
    IExpression<SimLogic<BigInteger>> keyExpr,
    IExpression<SimLogic<BigInteger>> rhsExpr) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var key = keyExpr.Evaluate().Value;
        var val = rhsExpr.Evaluate();
        aa[key].AssignFromBigInteger(val.Value, val.Unknown);
        yield break;
    }
}