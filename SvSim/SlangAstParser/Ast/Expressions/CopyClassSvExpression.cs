namespace SvSim.SlangAstParser.Ast.Expressions;

public record CopyClassSvExpression : SvExpression
{
    public SvExpression? SourceExpr;
};