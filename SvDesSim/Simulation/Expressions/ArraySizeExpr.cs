using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class ArraySizeExpr<T>(object targetObj) : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var size = targetObj switch
        {
            DynamicArrayVar<ISimLogicSignal> dyn => dyn.Size,
            QueueVar<ISimLogicSignal> q => q.Size,
            AssociativeArrayVar<BigInteger, ISimLogicSignal> aa => aa.Size,
            _ => 0
        };
        return new SimLogic<T>(T.CreateTruncating(size), T.Zero);
    }
}