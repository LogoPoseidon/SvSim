namespace SvDesSim.Simulation.Expressions;

public class StringLiteralExpr(string value) : IExpression<string>
{
    public string Evaluate() => value;
}