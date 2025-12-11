namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record AssociativeArrayType : SvType
{
    public SvType? ElementType;
    public SvType? IndexType;
};