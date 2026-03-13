using System.Numerics;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class SignalCastReadExpr<T>(ISimLogicSignal signal) : IExpression<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate() => signal.ReadAsLogic<T>();
}