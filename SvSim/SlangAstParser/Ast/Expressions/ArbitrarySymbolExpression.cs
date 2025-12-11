using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ArbitrarySymbolExpression : Expression
{
    public required SvSymbol Symbol;
    public HierarchicalReference? HierRef;
};