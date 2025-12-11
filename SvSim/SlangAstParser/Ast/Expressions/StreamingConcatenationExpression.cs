namespace SvSim.SlangAstParser.Ast.Expressions;

public record StreamingConcatenationExpression : Expression
{
    public bool? FixedSize;
    public ulong? BitstreamWidth;
    public ulong? SliceSize;
    public StreamExpression[] Streams = [];
};

public struct StreamExpression
{
    public required Expression Operand;
    public Expression? WithExpr;
    public long? ConstantWithWidth;
}