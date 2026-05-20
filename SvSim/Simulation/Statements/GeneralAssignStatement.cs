using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class GeneralAssignStatement(ISimLogicSignal lhs, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = rhs.Evaluate();
        lhs.AssignFromBigInteger(val.Value, val.Unknown);
        yield break;
    }
}