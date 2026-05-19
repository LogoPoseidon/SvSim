using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Signal;

public interface ISimEventSource {
    void Subscribe(ISimEvent consumer);
    void Unsubscribe(ISimEvent consumer);
}