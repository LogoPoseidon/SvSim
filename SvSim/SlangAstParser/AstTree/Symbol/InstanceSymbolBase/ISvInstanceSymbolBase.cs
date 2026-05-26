using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.TimingControl;

namespace SvSim.SlangAstParser.AstTree.Symbol.InstanceSymbolBase;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvCheckerInstance), nameof(SvSymbolKind.CheckerInstance))]
[JsonDerivedType(typeof(SvInstance), nameof(SvSymbolKind.Instance))]
[JsonDerivedType(typeof(SvPrimitiveInstance), nameof(SvSymbolKind.PrimitiveInstance))]
public interface ISvInstanceSymbolBase : ISvSymbol;

public record SvCheckerInstance : ISvInstanceSymbolBase
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvInstanceBody? Body { get; init; }
    public InstanceConnection[]? Connections { get; init; }
    public string? Kind { get; init; }
}
public record SvInstance : ISvInstanceSymbolBase
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvInstanceBody? Body { get; init; }
    public InstanceConnection[]? Connections { get; init; }
    public string? Kind { get; init; }
}
public record InstanceConnection(
    ISvSymbol? Port,
    ISvExpression? Expr,
    string? IfaceInstance,
    string? Modport,
    string? Formal,
    ISvExpression? Actual);
public record SvPrimitiveInstance : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? PrimitiveType { get; init; }
    public ISvExpression[]? Ports { get; init; }
    public ISvTimingControl? Delay { get; init; }
    public string? Kind { get; init; }
}