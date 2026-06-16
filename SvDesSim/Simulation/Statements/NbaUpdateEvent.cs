using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NbaUpdateEvent<T>(SimVar<T> lhs, T value) : ISimEvent 
    where T : IEquatable<T>
{
    public void Execute() => lhs.Assign(value);
    public void Trigger() {}
}