using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record UnpackedStructType : SvType
{
    public FieldSymbol[] Fields = [];
    public ulong? SelectableWidth;
    public ulong? BitstreamWidth;
    public int? SystemId;
};