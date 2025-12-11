namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record InvalidAssertionExpr : AssertionExpr
{
    public required AssertionExpr Child;
}