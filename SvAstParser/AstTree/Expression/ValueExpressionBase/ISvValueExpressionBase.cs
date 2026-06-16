using System.Text.Json.Serialization;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol;

namespace SvAstParser.AstTree.Expression.ValueExpressionBase;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvHierarchicalValue), nameof(SvExpressionKind.HierarchicalValue))]
[JsonDerivedType(typeof(SvNamedValue), nameof(SvExpressionKind.NamedValue))]
public interface ISvValueExpressionBase : ISvExpression
{
    public string Symbol { get; init; }
    [JsonIgnore] public ISvSymbol? ResolvedSymbol { get; set; }
}
public record SvHierarchicalValue : ISvValueExpressionBase
{
    public required string Type { get; init; }
    public required string Symbol { get; init; }
    public ISvSymbol? ResolvedSymbol { get; set; }
    public string? Ref { get; init; }
    public string? Kind { get; init; }
}
public record SvNamedValue : ISvValueExpressionBase
{
    public required string Type { get; init; }
    public required string Symbol { get; init; }
    public ISvSymbol? ResolvedSymbol { get; set; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}