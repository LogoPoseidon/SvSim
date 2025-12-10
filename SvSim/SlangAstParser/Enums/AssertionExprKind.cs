namespace SvSim.SlangAstParser.Enums;

public enum AssertionExprKind
{
    Invalid,
    Simple,
    SequenceConcat,
    SequenceWithMatch,
    Unary,
    Binary,
    FirstMatch,
    Clocking,
    StrongWeak,
    Abort,
    Conditional,
    Case,
    DisableIff
}