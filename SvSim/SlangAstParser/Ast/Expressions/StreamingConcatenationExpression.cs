namespace SvSim.SlangAstParser.Ast.Expressions;

public record StreamingConcatenationExpression : SvExpression
{
    public bool? FixedSize;
    public ulong? BitstreamWidth;
    public ulong? SliceSize;
    public StreamExpression[] Streams = [];
};

public struct StreamExpression
{
    public required SvExpression Operand;
    public SvExpression? WithExpr;
    public long? ConstantWithWidth;
}