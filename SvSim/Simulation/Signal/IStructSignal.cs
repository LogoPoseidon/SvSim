namespace SvSim.Simulation.Signal;

public interface IStructSignal : ISimLogicSignal
{
    string StructTypeName { get; set; }
}