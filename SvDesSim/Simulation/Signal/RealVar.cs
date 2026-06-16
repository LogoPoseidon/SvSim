namespace SvDesSim.Simulation.Signal;

public sealed class RealVar(double initialValue = 0.0) 
    : SimVar<double>(64, initialValue)
{
    protected override double ApplyMask(double value) => value;
}

public sealed class TracedRealVar(
    string name, 
    string vcdId, 
    double initialValue = 0.0, 
    Action<ITraceableSignal>? onDirty = null)
    : TracedVar<double>(name, vcdId, 64, initialValue, onDirty!)
{
    public override string GetVcdValueString()
    {
        return "r" + Value.ToString("G17", System.Globalization.CultureInfo.InvariantCulture);
    }

    protected override double ApplyMask(double value) => value;
}