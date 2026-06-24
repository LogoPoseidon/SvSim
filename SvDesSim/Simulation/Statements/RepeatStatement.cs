using System.Numerics;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class RepeatStatement(IExpression<SimLogic<BigInteger>> countExpr, IStatement body) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var count = (long)countExpr.Evaluate().Value;
        for (long i = 0; i < count; i++)
        {
            var shouldBreak = false;
            using (var enumerator = body.Execute().GetEnumerator())
            {
                while (true)
                {
                    YieldInstruction current;
                    try
                    {
                        if (!enumerator.MoveNext()) break;
                        current = enumerator.Current;
                    }
                    catch (BreakException)
                    {
                        shouldBreak = true;
                        break;
                    }
                    catch (ContinueException)
                    {
                        break;
                    }
                    yield return current;
                }
            }
            if (shouldBreak) break;
        }
    }
}
