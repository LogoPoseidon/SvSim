using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ArbitrarySymbolSvExpression : SvExpression
{
    public required SvSymbol Symbol;
    public HierarchicalReference? HierRef;
};