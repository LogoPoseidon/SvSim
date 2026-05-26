using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol;

namespace SvSim.SlangAstParser.AstTree.Expression.ValueExpressionBase;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvHierarchicalValue), nameof(SvExpressionKind.HierarchicalValue))]
[JsonDerivedType(typeof(SvNamedValue), nameof(SvExpressionKind.NamedValue))]
public interface ISvValueExpressionBase : ISvExpression
{
    public ISvSymbol Symbol { get; init; }
}
public record SvHierarchicalValue : ISvExpression
{
    public required string Type { get; init; }
    public required string Symbol { get; init; }
    public string? Ref { get; init; }
    public string? Kind { get; init; }
}
public record SvNamedValue : ISvExpression
{
    public required string Type { get; init; }
    public required string Symbol { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}