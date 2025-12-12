using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ArbitrarySymbolExpression : SvExpression
{
    public required SvSymbol Symbol;
    public HierarchicalReference? HierRef;
};