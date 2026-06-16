using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class AssignStatement<T>(SimVar<T> lhs, IExpression<T> rhs) : IStatement 
    where T : IEquatable<T>
{
    public IEnumerable<YieldInstruction> Execute()
    {
        lhs.Assign(rhs.Evaluate());
        yield break;
    }
}