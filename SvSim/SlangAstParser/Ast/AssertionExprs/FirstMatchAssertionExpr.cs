using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record FirstMatchAssertionExpr : AssertionExpr
{
    public required AssertionExpr Seq;
    public SvExpression[] MatchItems = [];
}