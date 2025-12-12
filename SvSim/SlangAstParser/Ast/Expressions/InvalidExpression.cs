namespace SvSim.SlangAstParser.Ast.Expressions;

public record InvalidExpression : SvExpression
{
    public SvExpression? Child;
};