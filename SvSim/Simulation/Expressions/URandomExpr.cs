using System.Numerics;
using SvSim.Simulation.RandomGenerator;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class URandomExpr<T> : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate() 
    {
        var randomVal = RandomGen.Random.Next();
        return new SimLogic<T>(T.CreateTruncating(randomVal), T.Zero);
    }
}