using System.Text.Json.Serialization;
using SvAstParser.AstTree.Symbol.Type;

namespace SvAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol.TempVarSymbol;

public interface ISvTempVarSymbol : ISvVariableSymbol
{
    public string? NextTmp { get; init; }
};

public record SvIterator : ISvTempVarSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public string? NextTmp { get; init; }
    [JsonIgnore] public ISvTempVarSymbol? ResolvedNextTmp { get; set; }
    public ISvType? ArrayType { get; init; }
    public string? IndexMethodName { get; init; }
}
public record SvPatternVar : ISvTempVarSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    [JsonIgnore] public ISvType? ResolvedType { get; set; }
    public string? NextTmp { get; init; }
    [JsonIgnore] public ISvTempVarSymbol? ResolvedNextTmp { get; set; }
    public string? Kind { get; init; }
}