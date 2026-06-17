using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class ConcatExpr<T>(List<(IExpression<SimLogic<BigInteger>> expr, int width)> operands) : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        BigInteger val = 0;
        BigInteger unk = 0;
        foreach (var (expr, width) in operands)
        {
            var ev = expr.Evaluate();
            var mask = (BigInteger.One << width) - 1;
            val = (val << width) | (ev.Value & mask);
            unk = (unk << width) | (ev.Unknown & mask);
        }
        return new SimLogic<T>(T.CreateTruncating(val), T.CreateTruncating(unk));
    }
}