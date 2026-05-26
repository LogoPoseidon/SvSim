using SvSim.SlangAstParser.AstTree.Symbol.Type;

namespace SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol.TempVarSymbol;

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
    public string? NextTmp { get; init; }
    public ISvType? ArrayType { get; init; }
    public string? IndexMethodName { get; init; }
}
public record SvPatternVar : ISvTempVarSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? NextTmp { get; init; }
    public string? Kind { get; init; }
}