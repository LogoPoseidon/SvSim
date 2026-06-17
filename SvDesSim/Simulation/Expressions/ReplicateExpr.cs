using System.Numerics;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class ReplicationExpr<T>(IExpression<SimLogic<BigInteger>> countExpr, IExpression<SimLogic<BigInteger>> innerExpr, int innerWidth) : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        var count = (int)countExpr.Evaluate().Value;
        var inner = innerExpr.Evaluate();
        
        BigInteger val = 0;
        BigInteger unk = 0;
        var mask = (BigInteger.One << innerWidth) - 1;
        
        for(var i = 0; i < count; i++)
        {
            val = (val << innerWidth) | (inner.Value & mask);
            unk = (unk << innerWidth) | (inner.Unknown & mask);
        }
        
        return new SimLogic<T>(T.CreateTruncating(val), T.CreateTruncating(unk));
    }
}