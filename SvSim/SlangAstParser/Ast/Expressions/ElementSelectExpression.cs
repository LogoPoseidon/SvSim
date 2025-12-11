namespace SvSim.SlangAstParser.Ast.Expressions;

public record ElementSelectExpression : Expression
{
    public Expression? Value;
    public Expression? Selector;
    public bool? IsConstantSelect;
};