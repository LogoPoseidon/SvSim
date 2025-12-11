namespace SvSim.SlangAstParser.Ast.Expressions;

public record NewArrayExpression : Expression
{
    public Expression? SizeExpr;
    public Expression? InitExpr;
};