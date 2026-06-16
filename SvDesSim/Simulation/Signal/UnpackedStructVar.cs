using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public class UnpackedStructVar(Dictionary<string, ISimLogicSignal> members) : ISimEventSource
{
    public Dictionary<string, ISimLogicSignal> Members { get; } = members;

    public void Subscribe(ISimEvent consumer)
    {
        foreach (var member in Members.Values)
            member.Subscribe(consumer);
    }

    public void Unsubscribe(ISimEvent consumer)
    {
        foreach (var member in Members.Values)
            member.Unsubscribe(consumer);
    }
}