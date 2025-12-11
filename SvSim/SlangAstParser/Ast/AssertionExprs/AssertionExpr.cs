using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public abstract record AssertionExpr : AstNode
{
    public AssertionExprKind Kind;
    public NondegeneracyCheckResult? NondegeneracyCheckResult;
    public NondegeneracyRequirement? NondegeneracyRequirement;
};

public struct NondegeneracyCheckResult
{
    public NondegeneracyStatus? Status;
    public required bool IsAlwaysFalse;
}

public enum NondegeneracyRequirement
{
    Default,
    OverlapOp,
    NonOverlapOp,
}