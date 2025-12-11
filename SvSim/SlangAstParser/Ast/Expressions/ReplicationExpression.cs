namespace SvSim.SlangAstParser.Ast.Expressions;

public record ReplicationExpression : Expression
{
    public Expression? Count;
    public Expression? Concat;
};