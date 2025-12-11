namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record SequenceConcatExpr : AssertionExpr
{
    public required Element[] Elements = [];
};

public struct Element
{
    public SequenceRange Delay;
    public SequenceRange DelayRange;
    public required AssertionExpr Sequence;
}