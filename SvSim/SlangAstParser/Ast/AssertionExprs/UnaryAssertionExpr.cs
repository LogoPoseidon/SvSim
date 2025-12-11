using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record UnaryAssertionExpr : AssertionExpr
{
    public UnaryAssertionOperator? Op;
    public AssertionExpr? Expr;
    public SequenceRange? Range;
};