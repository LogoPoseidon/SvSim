using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ImmediateAssertionStatement : SvStatement
{
    public SvExpression? Cond;
    public SvStatement? IfTrue;
    public SvStatement? IfFalse;
    public AssertionKind? AssertionKind;
    public bool? IsDeferred;
    public bool? IsFinal;
};