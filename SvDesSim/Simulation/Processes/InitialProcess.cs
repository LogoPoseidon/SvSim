using JetBrains.Annotations;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Statements;

namespace SvDesSim.Simulation.Processes;

public class InitialProcess
{
    public InitialProcess(IStatement block, EventScheduler scheduler)
    {
        var routine = new SvProcess(ExecuteBlock(block), scheduler);
        
        routine.Start();
    }

    [MustDisposeResource]
    private static IEnumerator<YieldInstruction> ExecuteBlock(IStatement block)
    {
        return block.Execute().GetEnumerator();
    }
}