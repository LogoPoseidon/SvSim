namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record EnumType : IntegralType
{
    public SvType? BaseType;
    public int? SystemId;
};