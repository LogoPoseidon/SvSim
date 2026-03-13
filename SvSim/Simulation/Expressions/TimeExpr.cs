using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Expressions;

public class TimeExpr<T>(EventScheduler scheduler) : IExpression<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        return new SimLogic<T>(T.CreateTruncating(scheduler.CurrentTime), T.Zero);
    }
}