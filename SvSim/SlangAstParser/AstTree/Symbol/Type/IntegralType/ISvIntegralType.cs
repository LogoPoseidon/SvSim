using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.Scope;

namespace SvSim.SlangAstParser.AstTree.Symbol.Type.IntegralType;

public interface ISvIntegralType : ISvType
{
    long BitWidth { get; init; }
    bool IsSigned { get; init; }
    bool IsFourState { get; init; }
}

public record SvEnum : ISvIntegralType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    [JsonIgnore] public ISvType? ResolvedBaseType { get; set; }
    public string? BaseType { get; init; }
    public int? SystemId { get; init; }
    public long BitWidth { get; init; }
    public bool IsSigned { get; init; }
    public bool IsFourState { get; init; }
    public string? Kind { get; init; }
}

public record SvPackedArray : ISvIntegralType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public long BitWidth { get; init; }
    public bool IsSigned { get; init; }
    public bool IsFourState { get; init; }
    public ISvType? ElementType { get; init; }
    public string? Kind { get; init; }
}

public record SvPackedStruct : ISvIntegralType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public long BitWidth { get; init; }
    public bool IsSigned { get; init; }
    public bool IsFourState { get; init; }
    public int SystemId { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvPackedUnion : ISvIntegralType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public long BitWidth { get; init; }
    public bool IsSigned { get; init; }
    public bool IsFourState { get; init; }
    public int SystemId { get; init; }
    public bool IsTagged { get; init; }
    public bool IsSoft { get; init; }
    public uint TagBits { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvPredefinedInteger : ISvIntegralType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public long BitWidth { get; init; }
    public bool IsSigned { get; init; }
    public bool IsFourState { get; init; }
    [JsonIgnore]
    public SvPredefinedIntegerKind IntegerKind => Name switch
    {
        "shortint" => SvPredefinedIntegerKind.ShortInt,
        "int" => SvPredefinedIntegerKind.Int,
        "longint" => SvPredefinedIntegerKind.LongInt,
        "byte" => SvPredefinedIntegerKind.Byte,
        "integer" => SvPredefinedIntegerKind.Integer,
        "time" => SvPredefinedIntegerKind.Time,
        _ => throw new InvalidOperationException($"Unknown predefined integer type: {Name}")
    };

    public string? Kind { get; init; }
}

public enum SvPredefinedIntegerKind
{
    ShortInt,
    Int,
    LongInt,
    Byte,
    Integer,
    Time
}

public record SvScalar : ISvIntegralType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public long BitWidth { get; init; }
    public bool IsSigned { get; init; }
    public bool IsFourState { get; init; }
    [JsonIgnore]
    public SvScalarKind ScalarKind => Name switch
    {
        "bit" => SvScalarKind.Bit,
        "logic" => SvScalarKind.Logic,
        "reg" => SvScalarKind.Reg,
        _ => throw new InvalidOperationException($"Unknown scalar type: {Name}")
    };

    public string? Kind { get; init; }
}

public enum SvScalarKind
{
    Bit,
    Logic,
    Reg
}