namespace SvSim.SlangAstParser.Ast.Expressions;

public record InvalidSvExpression : SvExpression
{
    public SvExpression? Child;
};