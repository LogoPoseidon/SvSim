namespace SvSim.SlangAstParser.AstTree.SvEnums;
[Flags]
public enum SvConstraintBlockFlags
{
    None = 0,
    Pure = 1 << 1,
    Static = 1 << 2,
    Extern = 1 << 3,
    ExplicitExtern = 1 << 4,
    Initial = 1 << 5,
    Extends = 1 << 6,
    Final = 1 << 7 
}