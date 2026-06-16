using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class GeneralAssignStatement(ISimLogicSignal lhs, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = rhs.Evaluate();
        lhs.AssignFromBigInteger(val.Value, val.Unknown);
        yield break;
    }
}