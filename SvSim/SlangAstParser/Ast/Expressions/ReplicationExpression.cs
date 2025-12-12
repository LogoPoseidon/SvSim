namespace SvSim.SlangAstParser.Ast.Expressions;

public record ReplicationExpression : SvExpression
{
    public SvExpression? Count;
    public SvExpression? Concat;
};