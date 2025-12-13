namespace SvSim.SlangAstParser.AstTree.SvEnums;
[Flags]
public enum SvLookupFlags
{
    None = 0,
    Type = 1 << 0,
    AllowDeclaredAfter = 1 << 1,
    DisallowWildcardImport = 1 << 2,
    NoUndeclaredError = 1 << 3,
    NoUndeclaredErrorIfUninstantiated = 1 << 4,
    AllowIncompleteForwardTypedefs = 1 << 5,
    NoSelectors = 1 << 6,
    AllowRoot = 1 << 7,
    AllowUnit = 1 << 8,
    IfacePortConn = 1 << 9,
    StaticInitializer = 1 << 10,
    TypeReference = 1 << 11,
    AlwaysAllowUpward = 1 << 12,
    DisallowUnitReferences = 1 << 13,
    AllowUnnamedGenerate = 1 << 14,
    ForceHierarchical = AllowDeclaredAfter | NoUndeclaredErrorIfUninstantiated 
}