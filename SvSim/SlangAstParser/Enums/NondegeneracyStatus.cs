namespace SvSim.SlangAstParser.Enums;

public enum NondegeneracyStatus
{
    None = 0,
    AdmitsEmpty = 1 << 0,
    AcceptsOnlyEmpty = 1 << 1,
    AdmitsNoMatch = 1 << 2
}