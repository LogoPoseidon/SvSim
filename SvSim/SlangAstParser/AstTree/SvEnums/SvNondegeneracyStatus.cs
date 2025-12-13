namespace SvSim.SlangAstParser.AstTree.SvEnums;

public enum SvNondegeneracyStatus
{
    None = 0,
    AdmitsEmpty = 1 << 0,
    AcceptsOnlyEmpty = 1 << 1,
    AdmitsNoMatch = 1 << 2
}