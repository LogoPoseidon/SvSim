namespace SvSim.SlangAstParser.Ast.Symbols;

public record NetType : SvSymbol
{
    public DeclaredType? DeclaredType;
    public NetKind? NetKind;
};

public enum NetKind
{
    Unknown,
    Wire,
    WAnd,
    WOr,
    Tri,
    TriAnd,
    TriOr,
    Tri0,
    Tri1,
    TriReg,
    Supply0,
    Supply1,
    UWire,
    Interconnect,
    UserDefined
}