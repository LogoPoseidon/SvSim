using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

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