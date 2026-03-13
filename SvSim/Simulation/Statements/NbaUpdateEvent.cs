using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class NbaUpdateEvent<T>(SimVar<T> lhs, T value) : ISimEvent 
    where T : IEquatable<T>
{
    public void Execute() => lhs.Assign(value);
    public void Trigger() {}
}