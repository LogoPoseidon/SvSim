using SvSim.SlangAstParser.Ast.AssertionExprs;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ConcurrentAssertionStatement : SvStatement
{
    public AssertionExpr? PropertySpec;
    public SvStatement? IfTrue;
    public SvStatement? IfFalse;
    public AssertionKind? AssertionKind;
};