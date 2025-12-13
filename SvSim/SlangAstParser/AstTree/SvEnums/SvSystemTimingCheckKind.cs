namespace SvSim.SlangAstParser.AstTree.SvEnums;

public enum SvSystemTimingCheckKind
{
    Unknown,
    Setup,
    Hold,
    SetupHold,
    Recovery,
    Removal,
    RecRem,
    Skew,
    TimeSkew,
    FullSkew,
    Period,
    Width,
    NoChange
}