namespace SvSim.SlangAstParser.Ast.Expressions;

public record ElementSelectExpression : SvExpression
{
    public SvExpression? Value;
    public SvExpression? Selector;
    public bool? IsConstantSelect;
};