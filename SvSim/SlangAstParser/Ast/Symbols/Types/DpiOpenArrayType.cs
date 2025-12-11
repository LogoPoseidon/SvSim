namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record DpiOpenArrayType : SvType
{
    public SvType? ElementType;
    public bool? IsPacked;
};