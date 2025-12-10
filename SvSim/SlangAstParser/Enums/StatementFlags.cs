namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum StatementFlags
{
    None = 0,
    InLoop = 1 << 0,
    InForkJoin = 1 << 1,
    InRandSeq = 1 << 2,
    HasTimingError = 1 << 3
}