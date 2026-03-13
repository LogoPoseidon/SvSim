using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

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
