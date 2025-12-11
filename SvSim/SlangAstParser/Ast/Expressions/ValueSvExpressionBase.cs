using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public abstract record ValueSvExpressionBase : SvExpression
{
    public ValueSymbol? Symbol;
};
public record HierarchicalValueSvExpression : ValueSvExpressionBase
{
    public HierarchicalReference? Ref;
}

public record NamedValueSvExpression : ValueSvExpressionBase
{
    
}