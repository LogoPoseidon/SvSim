namespace SvDesSim.Simulation.Expressions;

public interface IExpression<T>
{
    T Evaluate();
}