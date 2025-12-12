using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public abstract record ValueExpressionBase : SvExpression
{
    public ValueSymbol? Symbol;
};
public record HierarchicalValueExpression : ValueExpressionBase
{
    public HierarchicalReference? Ref;
}

public record NamedValueExpression : ValueExpressionBase
{
    
}