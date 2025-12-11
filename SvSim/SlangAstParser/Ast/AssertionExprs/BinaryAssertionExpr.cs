using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record BinaryAssertionExpr : AssertionExpr
{
    public required BinaryAssertionOperator Op;
    public required AssertionExpr Left;
    public required AssertionExpr Right;
};