using System.Text.Json.Serialization;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.Symbol;

namespace SvAstParser.AstTree.RandSeqProductionProd;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvRandSeqItem), "Item")]
[JsonDerivedType(typeof(SvRandSeqCodeBlock), "CodeBlock")]
[JsonDerivedType(typeof(SvRandSeqIfElse), "IfElse")]
[JsonDerivedType(typeof(SvRandSeqCase), "Case")]
[JsonDerivedType(typeof(SvRandSeqRepeat), "Repeat")]
public interface IRandSeqProductionProd : ISvAstNode;

public record SvRandSeqItem : IRandSeqProductionProd
{
    public required RandSeqItemDetails Item { get; init; }
    public string? Kind { get; init; }
}

public record RandSeqItemDetails
{
    public string? Target { get; init; }
    [JsonIgnore] public SvRandSeqProduction? ResolvedTarget { get; set; }
    public required ISvExpression[] Args { get; init; }
}

public record SvRandSeqCodeBlock : IRandSeqProductionProd
{
    public string? Kind { get; init; }
}

public record SvRandSeqIfElse : IRandSeqProductionProd
{
    public required ISvExpression Expr { get; init; }
    public required RandSeqItemDetails IfItem { get; init; }
    public RandSeqItemDetails? ElseItem { get; init; }
    public string? Kind { get; init; }
}

public record SvRandSeqCase : IRandSeqProductionProd
{
    public required ISvExpression Expr { get; init; }
    public RandSeqItemDetails? DefaultItem { get; init; }
    public required RandSeqCaseItem[] Items { get; init; }
    public string? Kind { get; init; }
}

public record RandSeqCaseItem
{
    public required ISvExpression[] Expressions { get; init; }
    public required RandSeqItemDetails Item { get; init; }
}

public record SvRandSeqRepeat : IRandSeqProductionProd
{
    public required ISvExpression Expr { get; init; }
    public required RandSeqItemDetails Item { get; init; }
    public string? Kind { get; init; }
}