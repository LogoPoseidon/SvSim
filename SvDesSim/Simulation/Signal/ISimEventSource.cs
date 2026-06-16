using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public interface ISimEventSource {
    void Subscribe(ISimEvent consumer);
    void Unsubscribe(ISimEvent consumer);
}