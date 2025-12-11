namespace SvSim.SlangAstParser.Ast.Expressions;

public record CopyClassExpression : Expression
{
    public Expression? SourceExpr;
};