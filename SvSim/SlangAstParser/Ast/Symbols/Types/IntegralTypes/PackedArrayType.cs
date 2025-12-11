namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record PackedArrayType : IntegralType
{
    public SvType? ElementType;
};