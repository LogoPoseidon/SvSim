namespace SvSim.SlangAstParser.AstTree.SvEnums;
[Flags]
public enum SvVariableFlags
{
    None = 0,
    Const = 1 << 0,
    CompilerGenerated = 1 << 1,
    ImmutableCoverageOption = 1 << 2,
    CoverageSampleFormal = 1 << 3,
    CheckerFreeVariable = 1 << 4,
    RefStatic = 1 << 5
}