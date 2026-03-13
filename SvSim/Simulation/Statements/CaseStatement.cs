using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class CaseStatement<T>(
    IExpression<SimLogic<T>> condition, 
    List<(IExpression<SimLogic<T>>[] matches, IStatement body)> items, 
    IStatement? defaultCase) : IStatement
    where T : IBinaryInteger<T>
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var condVal = condition.Evaluate();

        foreach (var item in from item in items from match in item.matches where condVal == match.Evaluate() select item)
        {
            foreach (var inst in item.body.Execute()) yield return inst;
            yield break;
        }

        if (defaultCase is null) yield break;
        {
            foreach (var inst in defaultCase.Execute()) yield return inst;
        }
    }
}