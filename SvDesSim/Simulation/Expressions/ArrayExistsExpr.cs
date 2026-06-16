using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class ArrayExistsExpr<T>(object targetObj, IExpression<SimLogic<BigInteger>> keyExpr) : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var key = keyExpr.Evaluate().Value;
        var exists = targetObj switch
        {
            AssociativeArrayVar<BigInteger, ISimLogicSignal> aa => aa.Exists(key),
            _ => false
        };
        return new SimLogic<T>(exists ? T.One : T.Zero, T.Zero);
    }
}