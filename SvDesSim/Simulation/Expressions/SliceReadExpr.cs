using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class SliceReadExpr<T>(ISimLogicSignal signal, int msb, int lsb) : IExpression<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var bigVal = signal.ReadSlice(msb, lsb);
        return new SimLogic<T>(T.CreateTruncating(bigVal.Value), T.CreateTruncating(bigVal.Unknown));
    }
}