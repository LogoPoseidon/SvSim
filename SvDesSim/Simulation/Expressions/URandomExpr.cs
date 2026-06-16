using System.Numerics;
using SvDesSim.Simulation.RandomGenerator;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class URandomExpr<T> : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate() 
    {
        var randomVal = RandomGen.Random.Next();
        return new SimLogic<T>(T.CreateTruncating(randomVal), T.Zero);
    }
}