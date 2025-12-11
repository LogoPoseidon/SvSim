namespace SvSim.SlangAstParser.Ast.Symbols;

public record InterfacePortSymbol : SvSymbol
{
    public DefinitionSymbol? InterfaceDef;
    public string? Modport;
    public bool? IsGeneric;
};