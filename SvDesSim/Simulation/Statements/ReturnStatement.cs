using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class ReturnStatement(IExpression<SimLogic<BigInteger>>? returnExpr) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = returnExpr?.Evaluate();
        throw new ReturnException(val);
    }
}