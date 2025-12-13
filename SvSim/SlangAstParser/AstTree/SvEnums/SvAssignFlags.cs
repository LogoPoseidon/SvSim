namespace SvSim.SlangAstParser.AstTree.SvEnums;
[Flags]
public enum SvAssignFlags
{
    None = 0,
    NonBlocking = 1 << 0,
    InConcat = 1 << 1,
    InOutPort = 1 << 2 
}