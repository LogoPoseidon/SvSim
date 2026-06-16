using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Engine;

public enum EventRegion
{
    Preponed, PreActive, Active, Inactive, PreNba, Nba, PostNba,
    PreObserved, Observed, PostObserved, Reactive, ReInactive,
    PreReNba, ReNba, PostReNba, PrePostponed, Postponed
}

public class EventScheduler
{
    public ulong CurrentTime { get; private set; }
    public event Action? OnPostponedStep;

    private readonly Queue<ISimEvent>[] _queues;
    private readonly PriorityQueue<(EventRegion Region, ISimEvent Action), ulong> _futureQueue = new();

    public EventScheduler()
    {
        _queues = new Queue<ISimEvent>[17];
        for (var i = 0; i < 17; i++)
        {
            _queues[i] = new Queue<ISimEvent>();
        }
    }

    public void Schedule(EventRegion region, ISimEvent action)
    {
        _queues[(int)region].Enqueue(action);
    }

    public void ScheduleFuture(ulong delayTicks, EventRegion region, ISimEvent action)
    {
        _futureQueue.Enqueue((region, action), CurrentTime + delayTicks);
    }

    public void Run()
    {
        while (true)
        {
            ExecuteTimeSlot();

            if (_futureQueue.TryPeek(out _, out var nextTime))
            {
                CurrentTime = nextTime;

                while (_futureQueue.TryPeek(out _, out var peekTime) && peekTime == CurrentTime)
                {
                    var ev = _futureQueue.Dequeue();
                    Schedule(ev.Region, ev.Action);
                }
            }
            else
            {
                break;
            }
        }
    }

    private void ExecuteTimeSlot()
    {
        Execute(EventRegion.Preponed);
        Execute(EventRegion.PreActive);

        while (HasAnyEvents(EventRegion.Active, EventRegion.PrePostponed))
        {
            while (HasAnyEvents(EventRegion.Active, EventRegion.PostObserved))
            {
                Execute(EventRegion.Active);
                
                var r = GetFirstNonEmptyRegion(EventRegion.Active, EventRegion.PostObserved);
                if (r.HasValue)
                {
                    SwapQueues(r.Value, EventRegion.Active);
                }
            }

            while (HasAnyEvents(EventRegion.Reactive, EventRegion.PostReNba))
            {
                Execute(EventRegion.Reactive);
                
                var r = GetFirstNonEmptyRegion(EventRegion.Reactive, EventRegion.PostReNba);
                if (r.HasValue)
                {
                    SwapQueues(r.Value, EventRegion.Reactive);
                }
            }

            if (!HasAnyEvents(EventRegion.Active, EventRegion.PostReNba))
            {
                Execute(EventRegion.PrePostponed);
            }
        }

        Execute(EventRegion.Postponed);

        OnPostponedStep?.Invoke();
    }

    private void Execute(EventRegion region)
    {
        var queue = _queues[(int)region];
        while (queue.TryDequeue(out var action))
        {
            action.Execute();
        }
    }

    /// <summary>
    /// O(1) Algorithm: Swaps the underlying queue references instead of dequeuing/enqueuing elements.
    /// This strictly maintains ordering while moving thousands of events instantly.
    /// </summary>
    private void SwapQueues(EventRegion source, EventRegion target)
    {
        var src = (int)source;
        var tgt = (int)target;
        
        (_queues[tgt], _queues[src]) = (_queues[src], _queues[tgt]);
    }
    
    private bool HasAnyEvents(EventRegion start, EventRegion end)
    {
        for (var i = (int)start; i <= (int)end; i++)
        {
            if (_queues[i].Count > 0) return true;
        }
        return false;
    }

    private EventRegion? GetFirstNonEmptyRegion(EventRegion start, EventRegion end)
    {
        for (var i = (int)start; i <= (int)end; i++)
        {
            if (_queues[i].Count > 0) return (EventRegion)i;
        }
        return null;
    }
}