namespace SvSim.SlangAstParser.AstTree.SvEnums;

public enum SvAssertionExprKind
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