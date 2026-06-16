using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NewArrayStatement(object? targetObj, IExpression<SimLogic<uint>> sizeExpr) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        if (targetObj is DynamicArrayVar<ISimLogicSignal> dynArr)
        {
            dynArr.New((int)sizeExpr.Evaluate().Value);
        }
        yield break;
    }
}