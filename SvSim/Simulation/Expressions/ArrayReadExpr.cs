using System.Numerics;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class ArrayReadExpr<T>(object container, IExpression<SimLogic<BigInteger>> indexExpr) : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var index = indexExpr.Evaluate().Value;

        switch (container)
        {
            case DynamicArrayVar<ISimLogicSignal> dynArr:
                return dynArr[(int)index].ReadAsLogic<T>();
            case QueueVar<ISimLogicSignal> queue:
                return queue[(int)index].ReadAsLogic<T>();
            case AssociativeArrayVar<BigInteger, ISimLogicSignal> aa:
                return aa[index].ReadAsLogic<T>();
            case ISimLogicSignal sig:
            {
                var slice = sig.ReadSlice((int)index, (int)index);
                return new SimLogic<T>(T.CreateTruncating(slice.Value), T.CreateTruncating(slice.Unknown));
            }
            default:
                return new SimLogic<T>(T.Zero, T.AllBitsSet);
        }
    }

}