namespace SvSim.SlangAstParser.Ast.Symbols.Types.IntegralTypes;

public record PackedUnionType : IntegralType
{
    public int? SystemId;
    public bool? IsTagged;
    public bool? IsSoft;
    public uint? TagBits;
};