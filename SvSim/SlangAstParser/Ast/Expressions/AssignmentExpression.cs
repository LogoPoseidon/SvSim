using SvSim.SlangAstParser.Ast.TimingControls;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record AssignmentExpression : SvExpression
{
    public BinaryOperator? Op;
    public TimingControl? TimingControl;
    public bool? IsCompound;
    public bool? IsNonBlocking;
    public bool? IsBlocking;
    public bool? IsLValueArg;
    public SvExpression? Left;
    public SvExpression? Right;
};