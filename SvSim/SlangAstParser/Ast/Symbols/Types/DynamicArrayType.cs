namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record DynamicArrayType : SvType
{
    public SvType? ElementType;
};