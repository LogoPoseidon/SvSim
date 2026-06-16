using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public class QueueVar<TVar>(Func<TVar> factory) : ISimEventSource where TVar : class
{
    public string ElementTypeName { get; set; } = "";

    private readonly List<TVar> _elements = [];
    private readonly HashSet<ISimEvent> _subscribers = [];

    public Func<TVar> Factory { get; } = factory;

    public int Size => _elements.Count;

    public TVar this[int index]
    {
        get => _elements[index];
        set
        {
            _elements[index] = value;
            Notify();
        }
    }

    public void PushBack(TVar item)
    {
        _elements.Add(item);
        Notify();
    }

    public void PushFront(TVar item)
    {
        _elements.Insert(0, item);
        Notify();
    }

    public TVar PopBack()
    {
        if (_elements.Count == 0) throw new InvalidOperationException("Queue is empty.");
        var idx = _elements.Count - 1;
        var val = _elements[idx];
        _elements.RemoveAt(idx);
        Notify();
        return val;
    }

    public TVar PopFront()
    {
        if (_elements.Count == 0) throw new InvalidOperationException("Queue is empty.");
        var val = _elements[0];
        _elements.RemoveAt(0);
        Notify();
        return val;
    }

    public void Delete()
    {
        _elements.Clear();
        Notify();
    }

    public void Subscribe(ISimEvent consumer) => _subscribers.Add(consumer);
    public void Unsubscribe(ISimEvent consumer) => _subscribers.Remove(consumer);

    private void Notify()
    {
        foreach (var sub in _subscribers) sub.Trigger();
    }
}