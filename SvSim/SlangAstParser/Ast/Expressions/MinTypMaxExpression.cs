namespace SvSim.SlangAstParser.Ast.Expressions;

public record MinTypMaxExpression : Expression
{
    public Expression? Min;
    public Expression? Typ;
    public Expression? Max;
    public Expression? Selected;
};