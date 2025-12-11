namespace SvSim.SlangAstParser.Ast.Symbols.Types.IntegralTypes;

public record ScalarType : IntegralType
{
    public ScalarKind Kind;
};
public enum ScalarKind
{
    Bit,
    Logic,
    Reg
}