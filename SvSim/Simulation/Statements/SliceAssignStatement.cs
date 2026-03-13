using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class SliceAssignStatement(ISimLogicSignal signal, int msb, int lsb, IExpression<SimLogic<BigInteger>> rhs) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var value = rhs.Evaluate();
        signal.WriteSlice(msb, lsb, value);
        yield break;
    }
}