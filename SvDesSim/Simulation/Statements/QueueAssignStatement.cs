using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class QueueAssignStatement(QueueVar<ISimLogicSignal> q, IExpression<SimLogic<BigInteger>> indexExpr, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var idx = (int)indexExpr.Evaluate().Value;
        var value = rhs.Evaluate();
        q[idx].AssignFromBigInteger(value.Value, value.Unknown);
        yield break;
    }
}