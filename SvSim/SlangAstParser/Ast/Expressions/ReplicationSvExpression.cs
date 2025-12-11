namespace SvSim.SlangAstParser.Ast.Expressions;

public record ReplicationSvExpression : SvExpression
{
    public SvExpression? Count;
    public SvExpression? Concat;
};