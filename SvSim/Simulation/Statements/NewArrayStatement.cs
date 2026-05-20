using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

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