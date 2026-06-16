using System.Numerics;
using SvDesSim.Simulation.RandomGenerator;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class URandomRangeExpr<T>(IExpression<SimLogic<T>> min, IExpression<SimLogic<T>> max) 
    : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    
    public SimLogic<T> Evaluate() 
    {
        var minVal = int.CreateTruncating(min.Evaluate().Value);
        var maxVal = int.CreateTruncating(max.Evaluate().Value);
        
        var randomVal = RandomGen.Random.Next(minVal, maxVal + 1);
        return new SimLogic<T>(T.CreateTruncating(randomVal), T.Zero);
    }
}