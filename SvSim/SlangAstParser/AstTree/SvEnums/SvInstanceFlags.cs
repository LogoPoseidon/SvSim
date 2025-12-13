namespace SvSim.SlangAstParser.AstTree.SvEnums;
[Flags]
public enum SvInstanceFlags
{
    None = 0,
    Uninstantiated = 1 << 0,
    FromBind = 1 << 1,
    ParentFromBind = 1 << 2,
    TargetedByBind = 1 << 3,
    PreventsCaching = Uninstantiated | FromBind | ParentFromBind
}