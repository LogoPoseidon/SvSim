using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record AbortAssertionExpr : AssertionExpr
{
    public Expression? Condition;
    public AssertionExpr? Expr;
    public bool? IsSync;
    public required Action Action;
}

public enum Action
{
    Accept,
    Reject,
}