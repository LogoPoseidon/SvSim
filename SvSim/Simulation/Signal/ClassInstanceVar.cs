using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Signal;

public class ClassInstanceVar(string typeName) : ISimEventSource
{
    public string ClassTypeName { get; } = typeName;
    public Dictionary<string, ISimLogicSignal> Properties { get; } = new();

    public ISimLogicSignal GetProperty(string name)
    {
        if (Properties.TryGetValue(name, out var sig)) return sig;
        
        var newSig = new LogicVar<uint>(32, new SimLogic<uint>(0, 0));
        Properties[name] = newSig;
        return newSig;
    }

    public void Subscribe(ISimEvent consumer) {}
    public void Unsubscribe(ISimEvent consumer) {}
}