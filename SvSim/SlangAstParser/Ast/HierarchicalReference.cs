using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast;

public record HierarchicalReference : AstNode
{
    public SvSymbol? Target;
    public SvExpression? Expression;
    public Element[] Path = [];
    public int? UpwardCount;
}

public struct Element
{
    public required SvSymbol Symbol;
    public Selector Selector;

}
public abstract record Selector;
public record IndexSelector(int Index) : Selector;
public record RangeSelector(int Start, int End) : Selector;
public record NameSelector(string Name) : Selector;