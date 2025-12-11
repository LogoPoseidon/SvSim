using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record RepeatedEventControl : TimingControl
{
    public SvExpression? Expr;
    public TimingControl? Event;
};