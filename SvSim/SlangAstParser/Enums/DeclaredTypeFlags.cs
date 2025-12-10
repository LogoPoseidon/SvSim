namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum DeclaredTypeFlags
{
    None = 0,
    InferImplicit = 1 << 0,
    InitializerCantSeeParent = 1 << 1,
    InitializerOverridden = 1 << 2,
    TypeOverridden = 1 << 3,
    AutomaticInitializer = 1 << 4,
    TypedefTarget = 1 << 5,
    NetType = 1 << 6,
    UserDefinedNetType = 1 << 7,
    FormalArgMergeVar = 1 << 8,
    Rand = 1 << 9,
    DPIReturnType = 1 << 10,
    DPIArg = 1 << 11,
    AllowUnboundedLiteral = 1 << 12,
    RequireSequenceType = 1 << 13,
    CoverageType = 1 << 14,
    InterconnectNet = 1 << 15,
    InterfaceVariable = 1 << 16,
    NeedsTypeCheck = NetType | UserDefinedNetType | FormalArgMergeVar | Rand | DPIReturnType |
                     DPIArg | RequireSequenceType | CoverageType | InterfaceVariable
}