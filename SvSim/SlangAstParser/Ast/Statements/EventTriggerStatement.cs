using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Ast.TimingControls;

namespace SvSim.SlangAstParser.Ast.Statements;

public record EventTriggerStatement : SvStatement
{
    public SvExpression? Target;
    public TimingControl? Timing;
    public bool? IsNonBlocking;
};