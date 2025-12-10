namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum AssignFlags
{
    None = 0,
    NonBlocking = 1 << 0,
    InConcat = 1 << 1,
    InOutPort = 1 << 2
}