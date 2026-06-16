using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class ForStatement(
    IStatement initializers, 
    IExpression<SimLogic<uint>> stopExpr, 
    IStatement steps, 
    IStatement body) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        foreach (var inst in initializers.Execute()) yield return inst;

        while (stopExpr.Evaluate().Value != 0)
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

                    yield return current;
                }
            }

            if (shouldBreak) break;

            foreach (var inst in steps.Execute()) yield return inst;
        }
    }
}