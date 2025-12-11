namespace SvSim.SlangAstParser.Ast.TimingControls;

public record EventListControl : TimingControl
{
    public TimingControl[] Events = [];
};