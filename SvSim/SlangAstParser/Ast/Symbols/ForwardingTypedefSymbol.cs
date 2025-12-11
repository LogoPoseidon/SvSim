using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record ForwardingTypedefSymbol : SvSymbol
{
    public ForwardTypeRestriction? TypeRestriction;
    public Visibility? Visibility;
};