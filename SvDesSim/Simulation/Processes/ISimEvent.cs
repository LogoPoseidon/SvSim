namespace SvDesSim.Simulation.Processes;

public interface ISimEvent
{
    void Execute();
    void Trigger();
}