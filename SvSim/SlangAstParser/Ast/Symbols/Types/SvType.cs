namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public abstract record SvType : SvSymbol
{
    public ulong? MaxBitWidth;
}