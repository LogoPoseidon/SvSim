using SvSim.Simulation.Signal;

namespace SvSim.Elaboration;

public class HierarchicalScope
{
    public string Name { get; }
    public string FullName { get; }
    public HierarchicalScope? Parent { get; }
    
    public Dictionary<string, ISimEventSource> Signals { get; } = new();
    
    public Dictionary<string, HierarchicalScope> Children { get; } = new();

    public HierarchicalScope(string name, HierarchicalScope? parent)
    {
        Name = name;
        Parent = parent;
        FullName = parent == null ? name : $"{parent.FullName}.{name}";
    }

    public void AddSignal(string name, ISimEventSource signal)
    {
        Signals[name] = signal;
    }

    public void AddChild(HierarchicalScope child)
    {
        Children[child.Name] = child;
    }
}