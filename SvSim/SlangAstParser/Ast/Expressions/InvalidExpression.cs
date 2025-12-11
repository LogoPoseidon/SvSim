namespace SvSim.SlangAstParser.Ast.Expressions;

public record InvalidExpression : Expression
{
    public Expression? Child;
};