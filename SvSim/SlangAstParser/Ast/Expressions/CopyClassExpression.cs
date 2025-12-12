namespace SvSim.SlangAstParser.Ast.Expressions;

public record CopyClassExpression : SvExpression
{
    public SvExpression? SourceExpr;
};