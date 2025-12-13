namespace SvSim.SlangAstParser.AstTree.SvEnums;
[Flags]
public enum SvCompilationFlags
{
    None = 0,
    AllowHierarchicalConst = 1 << 0,
    RelaxEnumConversions = 1 << 1,
    AllowUseBeforeDeclare = 1 << 2,
    AllowTopLevelIfacePorts = 1 << 3,
    LintMode = 1 << 4,
    IgnoreUnknownModules = 1 << 5,
    RelaxStringConversions = 1 << 6,
    AllowRecursiveImplicitCall = 1 << 7,
    AllowBareValParamAssignment = 1 << 8,
    AllowSelfDeterminedStreamConcat = 1 << 9,
    AllowMergingAnsiPorts = 1 << 10,
    DisableInstanceCaching = 1 << 11,
    DisallowRefsToUnknownInstances = 1 << 12,
    AllowUnnamedGenerate = 1 << 13
}