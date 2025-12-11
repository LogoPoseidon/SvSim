using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record SignalEventControl : TimingControl
{
    public SvExpression? Expr;
    public SvExpression? IffCondition;
    public EdgeKind? Edge;
};