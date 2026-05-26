using System.Numerics;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class ConditionalOpExpr<T>(
    IExpression<SimLogic<T>> cond, 
    IExpression<SimLogic<T>> left, 
    IExpression<SimLogic<T>> right) : IExpression<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var condVal = cond.Evaluate();
        if (condVal.Unknown != T.Zero)
        {
            return left.Evaluate() | right.Evaluate();
        }
        return condVal.Value != T.Zero ? left.Evaluate() : right.Evaluate();
    }
}