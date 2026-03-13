namespace SvSim.Simulation.Expressions;

public class BinaryOpExpr<T>(IExpression<T> left, IExpression<T> right, Func<T, T, T> operation) : IExpression<T>
{
    public T Evaluate() => operation(left.Evaluate(), right.Evaluate());
}