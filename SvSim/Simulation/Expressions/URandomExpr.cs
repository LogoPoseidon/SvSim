using System.Numerics;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class URandomExpr<T> : IExpression<SimLogic<T>> where T : IBinaryInteger<T>
{
    private static readonly Random Rnd = new();
    
    public SimLogic<T> Evaluate() 
    {
        var randomVal = Rnd.Next();
        return new SimLogic<T>(T.CreateTruncating(randomVal), T.Zero);
    }
}