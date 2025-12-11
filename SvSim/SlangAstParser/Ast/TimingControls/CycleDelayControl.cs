using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record CycleDelayControl : TimingControl
{
    public Expression? Expr;
};