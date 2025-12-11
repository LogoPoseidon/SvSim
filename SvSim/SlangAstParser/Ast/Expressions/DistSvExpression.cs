namespace SvSim.SlangAstParser.Ast.Expressions;

public record DistSvExpression : SvExpression
{
    public SvExpression? Left;
    public DistItem[] Items = [];
    public DistWeight? DefaultWeight;
    public struct DistItem
    {
        public SvExpression? Value;
        public DistWeight? Weight;
    }

    public struct DistWeight
    {
        public Kind? Kind;
        public SvExpression? Expr;
    }

};


public enum Kind
{
    PerValue,
    PerRange
}