namespace SvSim.Simulation.Expressions;

public interface IExpression<T>
{
    T Evaluate();
}