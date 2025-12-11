using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

public record FieldSymbol : VariableSymbol
{
    public ulong? BitOffset;
    public uint? FieldIndex;
    public RandMode? RandMode;
};