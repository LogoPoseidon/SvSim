using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class IfStatement(IExpression<SimLogic<uint>> condition, IStatement ifTrue, IStatement? ifFalse) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        if (condition.Evaluate().Value != 0)
        {
            foreach (var inst in ifTrue.Execute()) yield return inst;
        }
        else if (ifFalse != null)
        {
            foreach (var inst in ifFalse.Execute()) yield return inst;
        }
    }
}
