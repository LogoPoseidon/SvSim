namespace PDesSimulator.Simulator;

using System.Text;

public class Kernel
{
    private static readonly Lazy<Kernel> LazyInstance = new(() => new Kernel());
    public static Kernel Instance => LazyInstance.Value;

    private readonly PriorityQueue<DesEvent, ulong> _queue = new();
    private ulong _delta;
    private bool _stop;
    private ulong _stopTime;

    private readonly Dictionary<ISignal, List<DesEvent>> _lut = new();
    private readonly HashSet<ISignal> _updateRequests = [];
    private readonly List<DesEvent> _processes = [];
    private readonly HashSet<DesEvent> _markedProcesses = [];

    private bool _vcdActive;
    private string _vcdFile = "";
    private readonly StringBuilder _vcd = new();
    private readonly Dictionary<ISignal, string> _vcdLut = new();
    private readonly Dictionary<ISignal, string> _lastVcdValues = new();

    public event Action? OnTimeStepComplete;

    private Kernel() { Reset(); }

    public ulong Time { get; private set; }

    public void RegisterProcess(DesEvent e, List<ISignal>? sensitivity = null)
    {
        if (sensitivity != null)
        {
            foreach (var sig in sensitivity)
            {
                if (!_lut.TryGetValue(sig, out var list))
                {
                    list = [];
                    _lut[sig] = list;
                }
                list.Add(e);
            }
        }
        _processes.Add(e);
    }

    public void UpdateRequest(ISignal sig) => _updateRequests.Add(sig);

    public void RegisterWait(ulong delay, Action action)
    {
        var e = new DesEvent { Time = Time + delay, Action = action };
        _queue.Enqueue(e, e.Time);
    }

    public void StartSimulation(ulong time)
    {
        _stopTime = time;
        _stop = true;
        StartSimulation();
    }

    public void StartSimulation()
    {
        foreach (var process in _processes) process.Action();

        while (true)
        {
            if (_updateRequests.Count > 0)
            {
                var localUpdates = _updateRequests.ToList();
                _updateRequests.Clear();

                foreach (var signal in localUpdates)
                {
                    signal.Update();
                    if (!_lut.TryGetValue(signal, out var events)) continue;
                    foreach (var process in events)
                    {
                        _markedProcesses.Add(process);
                    }
                }
            }

            if (_markedProcesses.Count > 0)
            {
                var localMarked = _markedProcesses.ToList();
                _markedProcesses.Clear();

                foreach (var process in localMarked)
                {
                    process.Action();
                }
                
                _delta++;
                if (_delta >= 1000)
                {
                    Console.WriteLine("Delta Limit Reached. Infinite combinational loop detected.");
                    Reset();
                    return;
                }
                continue;
            }

            var isTimeStepDone = (_queue.Count == 0 || _queue.Peek().Time != Time);
            if (isTimeStepDone)
            {
                OnTimeStepComplete?.Invoke();
                if (_vcdActive) VcdSignals();
            }
            if (_vcdActive) VcdSignals();

            if (_queue.Count != 0)
            {
                var e = _queue.Dequeue();
                Time = e.Time;
                _delta = 0;

                if (_stop && Time > _stopTime)
                {
                    Console.WriteLine("Simulation stopped.");
                    Reset();
                    break;
                }

                e.Action();
            }
            else
            {
                Console.WriteLine("Simulation ended (no more events).");
                Reset();
                break;
            }
        }
    }

    public void Reset()
    {
        if (_vcdActive) VcdSave();
        Time = 0;
        _delta = 0;
        _stop = false;
        _stopTime = 0;
        _lut.Clear();
        _updateRequests.Clear();
        _processes.Clear();
        _markedProcesses.Clear();
        _vcd.Clear();
        _vcdLut.Clear();
        _lastVcdValues.Clear();
        _vcdActive = false;
        _queue.Clear();
        OnTimeStepComplete = null;
    }

    public void VcdInit(string file, IEnumerable<ISignal> allSignals, Predicate<ISignal>? filter = null)
    {
        _vcdActive = true;
        _vcdFile = file;
        _vcd.AppendLine("$timescale 1ns $end");
        _vcd.AppendLine("$scope module logic $end");

        var symbolId = 33; 
        foreach (var sig in allSignals)
        {
            if (filter != null && !filter(sig)) continue;
            var idStr = symbolId > 126 ? $"s{symbolId}" : ((char)symbolId).ToString();

            _vcd.AppendLine($"$var wire {sig.Width} {idStr} {sig.Name} $end");
            _vcdLut[sig] = idStr;
            _lastVcdValues[sig] = ""; 
            symbolId++;
        }
        _vcd.AppendLine("$upscope $end");
        _vcd.AppendLine("$enddefinitions $end");
        
        _vcd.AppendLine("#0");
        foreach (var sig in _vcdLut.Keys)
        {
            var val = sig.ToString();
            _vcd.AppendLine($"{val}{_vcdLut[sig]}");
            _lastVcdValues[sig] = val;
        }
    }

    private void VcdSignals()
    {
        var timeHeaderWritten = false;
        foreach (var sig in _vcdLut.Keys)
        {
            var val = sig.ToString();
            if (_lastVcdValues[sig] == val) continue;
            if (!timeHeaderWritten)
            {
                _vcd.AppendLine($"#{Time}");
                timeHeaderWritten = true;
            }
            _vcd.AppendLine($"{val}{_vcdLut[sig]}");
            _lastVcdValues[sig] = val;
        }
    }

    private void VcdSave()
    {
        try
        {
            var dir = Path.GetDirectoryName(_vcdFile);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(_vcdFile + ".vcd", _vcd.ToString());
        }
        catch
        {
            Console.WriteLine("Failed to write vcd.");
        }
    }
}