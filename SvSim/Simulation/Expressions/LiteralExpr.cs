namespace SvSim.Simulation.Expressions;

public class LiteralExpr<T>(T value) : IExpression<T>
{
    public T Evaluate() => value;
}
