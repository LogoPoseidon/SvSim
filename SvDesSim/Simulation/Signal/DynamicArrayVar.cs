using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public class DynamicArrayVar<TVar>(Func<TVar> factory) : ISimEventSource where TVar : class
{
    public string ElementTypeName { get; init; } = "";

    private TVar[] _elements = [];
    private readonly HashSet<ISimEvent> _subscribers = [];

    private Func<TVar> Factory { get; } = factory;

    public int Size => _elements.Length;

    public TVar this[int index]
    {
        get => _elements[index];
        set
        {
            _elements[index] = value;
            Notify();
        }
    }

    public void New(int size)
    {
        _elements = new TVar[size];
        for (var i = 0; i < size; i++)
        {
            _elements[i] = Factory();
        }
        Notify();
    }

    public void Delete()
    {
        _elements = [];
        Notify();
    }

    public void Subscribe(ISimEvent consumer) => _subscribers.Add(consumer);
    public void Unsubscribe(ISimEvent consumer) => _subscribers.Remove(consumer);

    private void Notify()
    {
        foreach (var sub in _subscribers) sub.Trigger();
    }

}