using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record CaseAssertionExpr : AssertionExpr
{
    public Expression? Expr;
    public ItemGroup[] Items = [];
    public AssertionExpr? DefaultCase;
};

public struct ItemGroup
{
    public Expression[] Expressions;
    public required AssertionExpr Body;
}