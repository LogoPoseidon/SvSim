namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record FloatingType : SvType
{
    public Kind Kind;
};

public enum Kind
{
    Real,
    ShortReal,
    RealTime
}