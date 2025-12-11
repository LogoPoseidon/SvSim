namespace SvSim.SlangAstParser.Ast;

public struct SequenceRepetition
{
    public required SequenceRange Range;
    public required Kind Kind;
}

public enum Kind
{
    Consecutive,
    NonConsecutive,
    GoTo
}