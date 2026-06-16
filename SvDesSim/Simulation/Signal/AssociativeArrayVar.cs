using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Signal;

public class AssociativeArrayVar<TKey, TVar>(Func<TVar> factory) : ISimEventSource
    where TKey : notnull
    where TVar : class
{
    public string ElementTypeName { get; init; } = "";

    private readonly Dictionary<TKey, TVar> _map = new();
    private readonly HashSet<ISimEvent> _subscribers = [];

    private Func<TVar> Factory { get; } = factory;

    public int Size => _map.Count;

    public TVar this[TKey key]
    {
        get
        {
            if (_map.TryGetValue(key, out var val)) return val;
            val = Factory();
            _map[key] = val;
            return val;
        }
        set
        {
            _map[key] = value;
            Notify();
        }
    }

    public bool Exists(TKey key) => _map.ContainsKey(key);

    public void Delete(TKey key)
    {
        if (_map.Remove(key)) Notify();
    }

    public void Clear()
    {
        _map.Clear();
        Notify();
    }

    public void Subscribe(ISimEvent consumer) => _subscribers.Add(consumer);
    public void Unsubscribe(ISimEvent consumer) => _subscribers.Remove(consumer);

    private void Notify()
    {
        foreach (var sub in _subscribers) sub.Trigger();
    }

}