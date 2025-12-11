using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record DelayControl : TimingControl
{
    public SvExpression? Expr;
};