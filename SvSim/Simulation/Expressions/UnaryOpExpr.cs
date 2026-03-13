namespace SvSim.Simulation.Expressions;

public class UnaryOpExpr<T>(IExpression<T> operand, Func<T, T> operation) : IExpression<T>
{
    public T Evaluate() => operation(operand.Evaluate());
}