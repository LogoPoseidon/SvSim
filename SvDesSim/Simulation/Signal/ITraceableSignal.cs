namespace SvDesSim.Simulation.Signal;

public interface ITraceableSignal
{
    string HierarchicalName { get; }
    string VcdId { get; }
    int BitWidth { get; }
    
    string GetVcdValueString(); 
    
    bool IsDirty { get; }
    void ClearDirty();
}