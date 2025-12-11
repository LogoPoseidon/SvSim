namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public abstract record IntegralType : SvType
{
    public uint? BitWidth;
    public bool? IsSigned;
    public bool? IsFourState;
};