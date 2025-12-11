using SvSim.SlangAstParser.Ast.TimingControls;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ClockingEventSvExpression : SvExpression
{
    public TimingControl? TimingControl;
};