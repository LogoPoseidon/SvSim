using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record DisableIffAssertionExpr : AssertionExpr
{
    public SvExpression? Condition;
    public AssertionExpr? Expr;
};