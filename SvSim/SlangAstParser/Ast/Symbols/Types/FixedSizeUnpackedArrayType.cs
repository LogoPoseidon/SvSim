namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record FixedSizeUnpackedArrayType : SvType
{
    public SvType? ElementType;
    public ulong SelectableWidth;
    public ulong BitstreamWidth;
};