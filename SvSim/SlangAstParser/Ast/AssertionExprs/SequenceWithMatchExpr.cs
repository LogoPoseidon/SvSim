using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record SequenceWithMatchExpr : AssertionExpr
{
    public required AssertionExpr Expr;
    public SequenceRepetition? Repetition;
    public Expression[] MatchItems = [];
};