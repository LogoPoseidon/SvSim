using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.TimingControl;

namespace SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;

public interface ISvVariableSymbol : ISvValueSymbol;

public record SvVariable : ISvVariableSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public SvVariableLifetime? Lifetime { get; init; }
    public ISvExpression? Initializer { get; init; }
    public string? Kind { get; init; }
    public SvVariableFlags Flags { get; init; }
    public SvAttribute[]? Attributes { get; init; }
}

public record SvClassProperty : ISvVariableSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public SvVariableLifetime? Lifetime { get; init; }
    public SvVisibility? Visibility { get; init; }
    public SvRandMode? RandMode { get; init; }
    public ISvExpression? Initializer { get; init; }
    public string? Flags { get; init; }
    public string? Kind { get; init; }
}

public record SvClockVar : ISvVariableSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public ISvExpression? Initializer { get; init; }
    public SvVariableLifetime? Lifetime { get; init; }
    public SvArgumentDirection? Direction { get; init; }
    public SvClockingSkew? OutputSkew { get; init; }
    public string? Kind { get; init; }
}
public record SvClockingSkew
{
    public ISvTimingControl? Delay { get; init; }
    public string? Edge { get; init; }
}

public record SvField : ISvVariableSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Kind { get; init; }
}

public record SvFormalArgument : ISvVariableSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public SvVariableLifetime? Lifetime { get; init; }
    public SvArgumentDirection? Direction { get; init; }
    public ISvExpression? DefaultValue { get; init; }
    public string? Flags { get; init; }
    public string? Kind { get; init; }
}

public record SvLocalAssertionVar : ISvVariableSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}