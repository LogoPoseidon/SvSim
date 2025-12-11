namespace SvSim.SlangAstParser.Ast.Expressions;

public record NewArraySvExpression : SvExpression
{
    public SvExpression? SizeExpr;
    public SvExpression? InitExpr;
};