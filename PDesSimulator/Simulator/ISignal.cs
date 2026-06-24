namespace PDesSimulator.Simulator;

public interface ISignal
{
    void Update();
    string Name { get; }
    int Width { get; }
    string ToString();
}