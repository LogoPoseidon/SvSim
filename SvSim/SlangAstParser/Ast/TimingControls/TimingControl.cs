using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.TimingControls;

public abstract record TimingControl : AstNode
{
    public TimingControlKind? Kind;
};