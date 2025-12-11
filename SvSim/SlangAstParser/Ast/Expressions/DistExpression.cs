namespace SvSim.SlangAstParser.Ast.Expressions;

public record DistExpression : Expression
{
    public Expression? Left;
    public DistItem[] Items = [];
    public DistWeight? DefaultWeight;
};

public struct DistItem
{
    public Expression? Value;
    public DistWeight? Weight;
}

public struct DistWeight
{
    public Kind? Kind;
    public Expression? Expr;
}

public enum Kind
{
    PerValue,
    PerRange
}