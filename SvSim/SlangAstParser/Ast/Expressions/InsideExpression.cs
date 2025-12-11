namespace SvSim.SlangAstParser.Ast.Expressions;

public record InsideExpression : Expression
{
    public Expression? Left;
    public Expression[] RangeList = [];
};