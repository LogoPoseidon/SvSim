namespace SvSim.SlangAstParser.Ast.Expressions;

public record NewArrayExpression : SvExpression
{
    public SvExpression? SizeExpr;
    public SvExpression? InitExpr;
};