using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record SignalEventControl : TimingControl
{
    public Expression? Expr;
    public Expression? IffCondition;
    public EdgeKind? Edge;
};