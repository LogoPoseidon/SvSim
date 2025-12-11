namespace SvSim.SlangAstParser.Ast.Symbols.InstanceSymbolBases;

public record InstanceSymbol : InstanceSymbolBase
{
    public ResolvedConfig? ResolvedConfig;
    public uint? InstanceDepth;
};