using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public record BlockEventListControl : TimingControl
{
    public Event[] Events = [];
};

public struct Event
{
    public SvExpression? Target;
    public bool? IsBegin;
}