using SvSim.SlangAstParser.Ast.TimingControls;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record ClockingAssertionExpr : AssertionExpr
{
    public TimingControl? Clocking;
    public required AssertionExpr Expr;
};