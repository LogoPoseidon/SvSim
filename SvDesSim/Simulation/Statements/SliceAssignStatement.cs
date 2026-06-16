using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class SliceAssignStatement(ISimLogicSignal signal, int msb, int lsb, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var value = rhs.Evaluate();
        signal.WriteSlice(msb, lsb, value);
        yield break;
    }
}