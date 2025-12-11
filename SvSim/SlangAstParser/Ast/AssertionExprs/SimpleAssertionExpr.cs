using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record SimpleAssertionExpr : AssertionExpr
{
    public required Expression Expr;
    public SequenceRepetition? Repetition;
    public bool? IsNullExpr;
};