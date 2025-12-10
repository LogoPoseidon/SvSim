namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum LookupResultFlags
{
    None = 0,
    WasImported = 1 << 0,
    IsHierarchical = 1 << 1,
    SuppressUndeclared = 1 << 2,
    FromTypeParam = 1 << 3,
    FromForwardTypedef = 1 << 4,
    IfacePort = 1 << 5 
}