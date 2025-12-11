using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record DisableIffAssertionExpr : AssertionExpr
{
    public Expression? Condition;
    public AssertionExpr? Expr;
};