using SvSim.SlangAstParser.Ast.TimingControls;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ClockingEventExpression : Expression
{
    public TimingControl? TimingControl;
};