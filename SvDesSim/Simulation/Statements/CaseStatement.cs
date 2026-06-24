using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

using SvDesSim.Elaboration;

namespace SvDesSim.Simulation.Statements;

public class CaseStatement<T>(
    IExpression<SimLogic<T>> condition, 
    List<(IExpression<SimLogic<T>>[] matches, IStatement body)> items, 
    IStatement? defaultCase) : IStatement
    where T : IBinaryInteger<T>
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var condVal = condition.Evaluate();

        foreach (var item in items)
        {
            foreach (var match in item.matches)
            {
                bool isMatch;
                if (match is RangeMatchExpr range)
                {
                    isMatch = range.IsMatch((SimLogic<BigInteger>)(object)condVal);
                }
                else
                {
                    isMatch = condVal == match.Evaluate();
                }

                if (!isMatch) continue;
                foreach (var inst in item.body.Execute()) yield return inst;
                yield break;
            }
        }

        if (defaultCase is null) yield break;
        {
            foreach (var inst in defaultCase.Execute()) yield return inst;
        }
    }
}