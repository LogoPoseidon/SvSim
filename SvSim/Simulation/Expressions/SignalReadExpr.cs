using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class SignalReadExpr<T>(SimVar<T> signal) : IExpression<T> where T : IEquatable<T>
{
    public T Evaluate() => signal.Value;
}