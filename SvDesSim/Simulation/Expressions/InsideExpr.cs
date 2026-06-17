using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public interface IInsideRange
{
    bool IsMatch(BigInteger leftValue);
}

public class SingleValueMatch(IExpression<SimLogic<BigInteger>> expr) : IInsideRange
{
    public bool IsMatch(BigInteger leftValue) => expr.Evaluate().Value == leftValue;
}

public class RangeValueMatch(IExpression<SimLogic<BigInteger>> min, IExpression<SimLogic<BigInteger>> max)
    : IInsideRange
{
    public bool IsMatch(BigInteger leftValue)
    {
        var minVal = min.Evaluate().Value;
        var maxVal = max.Evaluate().Value;
        return leftValue >= minVal && leftValue <= maxVal;
    }
}

public class InsideExpr<T>(IExpression<SimLogic<BigInteger>> left, List<IInsideRange> ranges)
    : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var leftVal = left.Evaluate().Value;
        return ranges.Any(r => r.IsMatch(leftVal)) ? new SimLogic<T>(T.One, T.Zero) : new SimLogic<T>(T.Zero, T.Zero);
    }
}