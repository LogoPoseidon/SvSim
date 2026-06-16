using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Vcd;

public class VcdTracer(string filePath)
{
    private readonly StreamWriter _writer = new(filePath);
    private readonly HashSet<ITraceableSignal> _dirtySignals = new();
    
    public void MarkDirty(ITraceableSignal signal)
    {
        _dirtySignals.Add(signal);
    }

    public void DumpTimeSlot(ulong currentTime)
    {
        if (_dirtySignals.Count == 0) return;

        _writer.WriteLine($"#{currentTime}");

        foreach (var signal in _dirtySignals)
        {
            var valStr = signal.GetVcdValueString();

            if (valStr.StartsWith('b') || valStr.StartsWith('r'))
            {
                _writer.WriteLine($"{valStr} {signal.VcdId}");
            }
            else
            {
                _writer.WriteLine($"{valStr}{signal.VcdId}");
            }

            signal.ClearDirty();
        }

        _dirtySignals.Clear();
    }
}