using System.Text.Json.Serialization;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol.Type;
using SvAstParser.AstTree.TimingControl;

namespace SvAstParser.AstTree.Symbol.ValueSymbol;

public interface ISvValueSymbol : ISvSymbol;

public record SvEnumValue : ISvValueSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Value { get; init; }
    public ISvExpression? Initializer { get; init; }
    public string? Kind { get; init; }
}

public record SvModportPort : ISvValueSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public SvArgumentDirection? Direction { get; init; }
    public string? InternalSymbol { get; init; }
    [JsonIgnore] public ISvSymbol? ResolvedInternalSymbol { get; set; }
    public string? Kind { get; init; }
}

public record SvNet : ISvValueSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public SvNetType? NetType { get; init; }
    public ISvExpression? Initializer { get; init; }
    public bool IsImplicit { get; init; }
    public string? Kind { get; init; }
    public ISvTimingControl? Delay { get; init; }
    public string? ExpansionHint { get; init; }
}

public record SvParameter : ISvValueSymbol // TODO ParameterSymbolBase
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public ISvExpression? Initializer { get; init; }
    public string? Value { get; init; }
    public bool IsLocal { get; init; }
    public bool IsPort { get; init; }
    public bool IsBody { get; init; }
    public string? Kind { get; init; }
}

public record SvPrimitivePort : ISvValueSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public SvPrimitiveDirection? Direction { get; init; }
    public string? Kind { get; init; }
}

public record SvSpecparam : ISvValueSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public ISvExpression? Initializer { get; init; }
    public bool IsPathPulse { get; init; }
    public string? Value { get; init; }
    public string? Kind { get; init; }
}