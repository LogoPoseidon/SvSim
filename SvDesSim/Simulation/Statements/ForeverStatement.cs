using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Statements;

public class ForeverStatement(IStatement body) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        while (true)
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
            
        }
    }
}