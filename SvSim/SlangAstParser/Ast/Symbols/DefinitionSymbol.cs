using SvSim.Shared;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record DefinitionSymbol : SvSymbol
{
    public NetType? NetType;
    public DefinitionKind? DefinitionKind;
    public VariableLifetime? DefaultLifetime;
    public UnconnectedDrive? UnconnectedDrive;
    public bool? CellDefine;
    public TimeScale? TimeScale;
    public ParameterDecl[] Parameters = [];
    public string[] Modports = [];
    public BindDirectiveInfo[] BindDirectives = [];
    public bool? HasNonAnsiPorts;
};

public struct ParameterDecl
{
    public string? Name;
    public bool? IsTypeParam;
    public bool? IsLocalParam;
    public bool? IsPortParam;
    public bool? HasSyntax;
}