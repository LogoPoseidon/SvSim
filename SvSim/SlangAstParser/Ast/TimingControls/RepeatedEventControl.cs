using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record RepeatedEventControl : TimingControl
{
    public Expression? Expr;
    public TimingControl? Event;
};