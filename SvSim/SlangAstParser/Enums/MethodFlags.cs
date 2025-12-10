namespace SvSim.SlangAstParser.Enums;
[Flags]
public enum MethodFlags
{
    None = 0,
    Virtual = 1 << 0,
    Pure = 1 << 1,
    Static = 1 << 2,
    Constructor = 1 << 3,
    InterfaceExtern = 1 << 4,
    ModportImport = 1 << 5,
    ModportExport = 1 << 6,
    DPIImport = 1 << 7,
    DPIContext = 1 << 8,
    BuiltIn = 1 << 9,
    Randomize = 1 << 10,
    ForkJoin = 1 << 11,
    DefaultedSuperArg = 1 << 12,
    Initial = 1 << 13,
    Extends = 1 << 14,
    Final = 1 << 15
}