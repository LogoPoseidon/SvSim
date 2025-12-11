namespace SvSim.SlangAstParser.Ast.Symbols.Types.IntegralTypes;

public record PredefinedIntegerType : IntegralType
{
    public PredefinedIntegerKind? Kind;
};

public enum PredefinedIntegerKind
{
    ShortInt,
    Int,
    LongInt,
    Byte,
    Integer,
    Time
}