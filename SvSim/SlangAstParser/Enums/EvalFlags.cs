namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum EvalFlags
{
    None = 0,
    IsScript = 1 << 0,
    CacheResults = 1 << 1,
    SpecparamsAllowed = 1 << 2,
    AllowUnboundedPlaceholder = 1 << 3
}