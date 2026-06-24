namespace PDesSimulator.Simulator;

public class DesEvent
{
    public ulong Time { get; init; }
    public required Action Action { get; init; }
}