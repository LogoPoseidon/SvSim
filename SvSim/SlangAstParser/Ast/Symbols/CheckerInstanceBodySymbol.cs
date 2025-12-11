using SvSim.SlangAstParser.Ast.Symbols.InstanceSymbolBases;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record CheckerInstanceBodySymbol : SvSymbol
{
    public CheckerInstanceSymbol? ParentInstance;
};