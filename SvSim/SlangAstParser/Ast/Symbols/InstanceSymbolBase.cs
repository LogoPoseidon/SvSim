namespace SvSim.SlangAstParser.Ast.Symbols;

public record InstanceSymbolBase : SvSymbol
{
    public uint[] ArrayPath = [];
};