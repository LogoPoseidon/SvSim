using System.Text.Json.Serialization;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.Scope;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol.InstanceSymbolBase;
using SvAstParser.AstTree.Symbol.Type.IntegralType;
using SvAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;
using SvAstParser.AstTree.TimingControl;

namespace SvAstParser.AstTree.Symbol.Type;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvEnum), nameof(SvTypeKind.EnumType))]
[JsonDerivedType(typeof(SvTypeAlias), nameof(SvTypeKind.TypeAlias))]
[JsonDerivedType(typeof(SvClassType), nameof(SvTypeKind.ClassType))]
[JsonDerivedType(typeof(SvPredefinedInteger), nameof(SvTypeKind.PredefinedIntegerType))]
[JsonDerivedType(typeof(SvScalar), nameof(SvTypeKind.ScalarType))]
[JsonDerivedType(typeof(SvFloatingType), nameof(SvTypeKind.FloatingType))]
[JsonDerivedType(typeof(SvPackedArray), nameof(SvTypeKind.PackedArrayType))]
[JsonDerivedType(typeof(SvFixedSizeUnpackedArrayType), nameof(SvTypeKind.FixedSizeUnpackedArrayType))]
[JsonDerivedType(typeof(SvDynamicArrayType), nameof(SvTypeKind.DynamicArrayType))]
[JsonDerivedType(typeof(SvDpiOpenArrayType), nameof(SvTypeKind.DPIOpenArrayType))]
[JsonDerivedType(typeof(SvAssociativeArrayType), nameof(SvTypeKind.AssociativeArrayType))]
[JsonDerivedType(typeof(SvQueueType), nameof(SvTypeKind.QueueType))]
[JsonDerivedType(typeof(SvPackedStruct), nameof(SvTypeKind.PackedStructType))]
[JsonDerivedType(typeof(SvUnpackedStructType), nameof(SvTypeKind.UnpackedStructType))]
[JsonDerivedType(typeof(SvPackedUnion), nameof(SvTypeKind.PackedUnionType))]
[JsonDerivedType(typeof(SvUnpackedUnionType), nameof(SvTypeKind.UnpackedUnionType))]
[JsonDerivedType(typeof(SvVoidType), nameof(SvTypeKind.VoidType))]
[JsonDerivedType(typeof(SvNullType), nameof(SvTypeKind.NullType))]
[JsonDerivedType(typeof(SvCHandleType), nameof(SvTypeKind.CHandleType))]
[JsonDerivedType(typeof(SvStringType), nameof(SvTypeKind.StringType))]
[JsonDerivedType(typeof(SvEventType), nameof(SvTypeKind.EventType))]
[JsonDerivedType(typeof(SvUnboundedType), nameof(SvTypeKind.UnboundedType))]
[JsonDerivedType(typeof(SvTypeRefType), nameof(SvTypeKind.TypeRefType))]
[JsonDerivedType(typeof(SvUntypedType), nameof(SvTypeKind.UntypedType))]
[JsonDerivedType(typeof(SvSequenceType), nameof(SvTypeKind.SequenceType))]
[JsonDerivedType(typeof(SvPropertyType), nameof(SvTypeKind.PropertyType))]
[JsonDerivedType(typeof(SvVirtualInterfaceType), nameof(SvTypeKind.VirtualInterfaceType))]
[JsonDerivedType(typeof(SvErrorType), nameof(SvTypeKind.ErrorType))]
public interface ISvType : ISvSymbol;

public record SvAssociativeArrayType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public required ISvType ElementType { get; init; }
    public ISvType? IndexType { get; init; }
    public string? Kind { get; init; }
}

public record SvTypeAlias : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Target { get; init; }
    [JsonIgnore] public ISvType? ResolvedTarget { get; set; }
    public SvVisibility Visibility { get; init; }
    public ISvSymbol? Forward { get; init; }
    public string? Kind { get; init; }
}

public record SvClassType : ISvType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public bool IsAbstract { get; init; }
    public bool IsInterface { get; init; }
    public bool IsFinal { get; init; }
    public string? BaseClass { get; init; }
    [JsonIgnore] public SvClassType? ResolvedBaseClass { get; set; }
    public string[]? Implements { get; init; }
    [JsonIgnore] public SvClassType[]? ResolvedImplements { get; set; }
    public string? GenericClass { get; init; }
    [JsonIgnore] public SvGenericClassDef? ResolvedGenericClass { get; set; }
    public string? Kind { get; init; }
    public ISvExpression? BaseConstructorCall { get; init; }
    public ISvSymbol? Forward { get; init; }
}

public record SvCovergroup : ISvType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public ISvTimingControl? Event { get; init; }
    public string? Kind { get; init; }
}

public record SvFloatingType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    [JsonIgnore]
    public FloatingTypeKind FloatingKind => Name switch
    {
        "real" => FloatingTypeKind.Real,
        "shortreal" => FloatingTypeKind.ShortReal,
        "realtime" => FloatingTypeKind.RealTime,
        _ => throw new InvalidOperationException($"Unknown floating type: {Name}")
    };

    public string? Kind { get; init; }
}

public enum FloatingTypeKind
{
    Real,
    ShortReal,
    RealTime
}

public record SvFixedSizeUnpackedArrayType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvType? ElementType { get; init; }
    public long SelectableWidth { get; init; }
    public long BitstreamWidth { get; init; }
    public string? Kind { get; init; }
}

public record SvDynamicArrayType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvType? ElementType { get; init; }
    public string? Kind { get; init; }
}

public record SvDpiOpenArrayType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvType? ElementType { get; init; }
    public bool IsPacked { get; init; }
    public string? Kind { get; init; }
}

public record SvQueueType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvType? ElementType { get; init; }
    public uint MaxBound { get; init; }
    public string? Kind { get; init; }
}

public record SvUnpackedStructType : ISvType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvField[]? Fields { get; init; }
    public ulong? SelectableWidth { get; init; }
    public ulong? BitStreamWidth { get; init; }
    public int SystemId { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvUnpackedUnionType : ISvType, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvField[]? Fields { get; init; }
    public ulong? SelectableWidth { get; init; }
    public ulong? BitStreamWidth { get; init; }
    public int SystemId { get; init; }
    public bool IsTagged { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvVoidType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvNullType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvCHandleType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvStringType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvEventType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvUnboundedType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvTypeRefType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvUntypedType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvSequenceType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvPropertyType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvVirtualInterfaceType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvInstance? Iface { get; init; }
    public string? Modport { get; init; }
    [JsonIgnore] public SvModport? ResolvedModport { get; set; }
    public bool IsRealIface { get; init; }
    public string? Kind { get; init; }
}

public record SvErrorType : ISvType
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}