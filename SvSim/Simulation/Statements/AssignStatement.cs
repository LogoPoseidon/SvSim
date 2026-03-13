using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class AssignStatement<T>(SimVar<T> lhs, IExpression<T> rhs) : IStatement 
    where T : IEquatable<T>
{
    public IEnumerable<YieldInstruction> Execute()
    {
        lhs.Assign(rhs.Evaluate());
        yield break;
    }
}