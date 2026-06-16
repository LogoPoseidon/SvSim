using System.Text.Json.Serialization;
using SvAstParser.AstTree.AssertionExpr;
using SvAstParser.AstTree.Constraint;
using SvAstParser.AstTree.Expression.AssignmentPatternExpression;
using SvAstParser.AstTree.Expression.ValueExpressionBase;
using SvAstParser.AstTree.Pattern;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.Symbol;
using SvAstParser.AstTree.TimingControl;

namespace SvAstParser.AstTree.Expression;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvAssignment), nameof(SvExpressionKind.Assignment))]
[JsonDerivedType(typeof(SvIntegerLiteral), nameof(SvExpressionKind.IntegerLiteral))]
[JsonDerivedType(typeof(SvConversion), nameof(SvExpressionKind.Conversion))]
[JsonDerivedType(typeof(SvNamedValue), nameof(SvExpressionKind.NamedValue))]
[JsonDerivedType(typeof(SvBinary), nameof(SvExpressionKind.BinaryOp))]
[JsonDerivedType(typeof(SvCall), nameof(SvExpressionKind.Call))]
[JsonDerivedType(typeof(SvDist), nameof(SvExpressionKind.Dist))]
[JsonDerivedType(typeof(SvElementSelect), nameof(SvExpressionKind.ElementSelect))]
[JsonDerivedType(typeof(SvInside), nameof(SvExpressionKind.Inside))]
[JsonDerivedType(typeof(SvRangeSelect), nameof(SvExpressionKind.RangeSelect))]
[JsonDerivedType(typeof(SvValueRange), nameof(SvExpressionKind.ValueRange))]
[JsonDerivedType(typeof(SvUnboundedLiteral), nameof(SvExpressionKind.UnboundedLiteral))]
[JsonDerivedType(typeof(SvSimpleAssignmentPattern), nameof(SvExpressionKind.SimpleAssignmentPattern))]
[JsonDerivedType(typeof(SvRealLiteral), nameof(SvExpressionKind.RealLiteral))]
[JsonDerivedType(typeof(SvStreamingConcatenation), nameof(SvExpressionKind.Streaming))]
[JsonDerivedType(typeof(SvMemberAccess), nameof(SvExpressionKind.MemberAccess))]
[JsonDerivedType(typeof(SvNewClass), nameof(SvExpressionKind.NewClass))]
[JsonDerivedType(typeof(SvUnbasedUnsizedIntegerLiteral), nameof(SvExpressionKind.UnbasedUnsizedIntegerLiteral))]
[JsonDerivedType(typeof(SvMinTypMax), nameof(SvExpressionKind.MinTypMax))]
[JsonDerivedType(typeof(SvConcatenation), nameof(SvExpressionKind.Concatenation))]
[JsonDerivedType(typeof(SvUnaryOp), nameof(SvExpressionKind.UnaryOp))]
[JsonDerivedType(typeof(SvHierarchicalValue), nameof(SvExpressionKind.HierarchicalValue))]
[JsonDerivedType(typeof(SvArbitrarySymbol), nameof(SvExpressionKind.ArbitrarySymbol))]
[JsonDerivedType(typeof(SvLValueReference), nameof(SvExpressionKind.LValueReference))]
[JsonDerivedType(typeof(SvStringLiteral), nameof(SvExpressionKind.StringLiteral))]
[JsonDerivedType(typeof(SvConditionalOp), nameof(SvExpressionKind.ConditionalOp))]
[JsonDerivedType(typeof(SvAssertionInstance), nameof(SvExpressionKind.AssertionInstance))]
[JsonDerivedType(typeof(SvEmptyArgument), nameof(SvExpressionKind.EmptyArgument))]
[JsonDerivedType(typeof(SvNewArray), nameof(SvExpressionKind.NewArray))]
[JsonDerivedType(typeof(SvNewCovergroup), nameof(SvExpressionKind.NewCovergroup))]
[JsonDerivedType(typeof(SvCopyClass), nameof(SvExpressionKind.CopyClass))]
[JsonDerivedType(typeof(SvTimeLiteral), nameof(SvExpressionKind.TimeLiteral))]
[JsonDerivedType(typeof(SvNullLiteral), nameof(SvExpressionKind.NullLiteral))]
[JsonDerivedType(typeof(SvReplication), nameof(SvExpressionKind.Replication))]
[JsonDerivedType(typeof(SvDataType), nameof(SvExpressionKind.DataType))]
[JsonDerivedType(typeof(SvTypeReference), nameof(SvExpressionKind.TypeReference))]
[JsonDerivedType(typeof(SvStructuredAssignmentPattern), nameof(SvExpressionKind.StructuredAssignmentPattern))]
[JsonDerivedType(typeof(SvReplicatedAssignmentPattern), nameof(SvExpressionKind.ReplicatedAssignmentPattern))]
[JsonDerivedType(typeof(SvClockingEvent), nameof(SvExpressionKind.ClockingEvent))]
[JsonDerivedType(typeof(SvTaggedUnion), nameof(SvExpressionKind.TaggedUnion))]
[JsonDerivedType(typeof(SvInvalid), nameof(SvExpressionKind.Invalid))]

public interface ISvExpression : ISvAstNode
{
    string Type { get; init; }
}

public record SvArbitrarySymbol : ISvExpression
{
    public required string Type { get; init; }
    public required string Symbol { get; init; }
    public string? Kind { get; init; }
}

public record SvAssertionInstance : ISvExpression
{
    public required string Type { get; init; }
    public required string Symbol { get; init; }
    public required ISvAssertionExpr Body { get; init; }
    public SvAssertionActualArg[]? Arguments { get; init; }
    public required AssertionInstanceLocalVar[] LocalVars { get; init; }
    public bool IsRecursiveProperty { get; init; }
    public string? Kind { get; init; }
}
public record SvAssertionActualArg(string Name, ISvExpression? Expression);
public record AssertionInstanceLocalVar
{
    public required string Name { get; init; }
    public SvSymbolKind? Kind { get; init; }
    public long? Addr { get; init; }
    public string? Type { get; init; }
    public string? Lifetime { get; init; }
    public ISvExpression? Value { get; init; }
}

public record SvAssignment : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Left { get; init; }
    public required ISvExpression Right { get; init; }
    public bool IsNonBlocking { get; init; }
    public bool IsBlocking { get; init; }
    public bool IsCompound { get; init; }
    public bool IsLValueArg { get; init; }
    public SvBinaryOperator? Op { get; init; }
    public ISvTimingControl? TimingControl { get; init; } // string?
    public string? Kind { get; init; }
}
public record SvBinary : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Left { get; init; }
    public required ISvExpression Right { get; init; }
    public SvBinaryOperator? Op { get; init; }
    public SvAttribute[]? Attributes { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvCall : ISvExpression
{
    public required string Type { get; init; }
    public ISvExpression? ThisClass { get; init; }
    public ISvExpression[]? Arguments { get; init; }
    public bool IsSystemCall { get; init; }
    public string? Subroutine { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
    public ISvConstraint? InlineConstraints { get; init; }
    public string[]? ConstraintRestrictions { get; init; }
    public ISvSymbol? IterVar { get; init; }
    public ISvExpression? IterExpr { get; init; }
}
public record SvClockingEvent : ISvExpression
{
    public required string Type { get; init; }
    public required ISvTimingControl TimingControl { get; init; }
    public string? Kind { get; init; }
}
public record SvConcatenation : ISvExpression
{
    public required string Type { get; init; }
    public ISvExpression[]? Operands { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvConditional : ISvExpression
{
public required string Type { get; init; }
public required ISvExpression Left { get; init; }
public required ISvExpression Right { get; init; }
public required string knownSide { get; init; }
public required SvConditionalExpressionCondition[] Conditions { get; init; }
public string? Kind { get; init; }
}
public struct SvConditionalExpressionCondition(string Expr, string? Pattern);

public record SvConversion : ISvExpression
{
    public required string Type { get; init; }
    public ISvExpression? Operand { get; init; }
    public bool IsImplicit { get; init; }
    public SvConversionKind ConversionKind { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvCopyClass : ISvExpression
{
    public required string Type { get; init; }
    public ISvExpression? SourceExpr { get; init; }
    public string? Kind { get; init; }
}
public record SvDataType : ISvExpression
{
    public required string Type { get; init; }
    public string? Kind { get; init; }
}
public record SvDist : ISvExpression
{
    public required string Type { get; init; }
    public ISvExpression? Left { get; init; }
    public required SvDistExpressionItem[] Items { get; init; }
    public string? DefaultWeight { get; init; } // TODO SvDistExpressionWeight?
    public string? Kind { get; init; }
}
public enum DistExpressionKind { PerValue, PerRange }

public struct SvDistExpressionItem
{
    public ISvExpression Value { get; init; }
    public ISvExpression? Weight { get; init; }
    public DistExpressionKind Kind { get; init; }

}
public record SvElementSelect : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Value { get; init; }
    public required ISvExpression Selector { get; init; }
    public bool IsConstantSelect { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; } 
}
public record SvEmptyArgument : ISvExpression
{
    public required string Type { get; init; }
    public string? Kind { get; init; }
}
public record SvInside : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Left { get; init; }
    public required ISvExpression[] RangeList { get; init; }
    public string? Kind { get; init; }
}
public record SvIntegerLiteral : ISvExpression
{
    public required string Type { get; init; }
    public string? Value { get; init; }
    public string? Constant { get; init; }
    public bool IsDeclaredUnsized { get; init; }
    public string? Kind { get; init; }
}
public record SvInvalid : ISvExpression
{
    public ISvExpression? Child { get; init; } 
    public required string Type { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvLValueReference : ISvExpression
{
    public required string Type { get; init; }
    public string? Kind { get; init; }
}
public record SvMemberAccess : ISvExpression
{
    public required string Type { get; init; }
    public required string Member { get; init; }
    public ISvExpression? Value { get; init; }
    public string? Kind { get; init; }
}
public record SvMinTypMax : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Selected { get; init; }
    public string? Kind { get; init; }
}
public record SvNewArray : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression SizeExpr { get; init; }
    public string? InitExpr { get; init; }
    public string? Kind { get; init; }
}
public record SvNewClass : ISvExpression
{
    public required string Type { get; init; }
    public bool IsSuperClass { get; init; }
    public ISvExpression? ConstructorCall { get; init; }
    public string? Kind { get; init; }
}
public record SvNewCovergroup : ISvExpression
{
    public required string Type { get; init; }
    public required string[] Arguments { get; init; }
    public string? Kind { get; init; }
}
public record SvNullLiteral : ISvExpression
{
    public required string Type { get; init; }
    public string? Kind { get; init; }
}
public record SvRangeSelect : ISvExpression
{
    public required string Type { get; init; }
    public SvRangeSelectionKind SelectionKind { get; init; }
    public required ISvExpression Value { get; init; }
    public required ISvExpression Left { get; init; }
    public required ISvExpression Right { get; init; }
    public bool IsConstantSelect { get; init; }
    public string? Kind { get; init; }
}
public record SvRealLiteral : ISvExpression
{
    public required string Type { get; init; }
    public double Value { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; } 
}
public record SvReplication : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Count { get; init; }
    public required ISvExpression Concat { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvStreamingConcatenation : ISvExpression
{
    public required string Type { get; init; }
    public bool IsFixedSize { get; init; }
    public int SliceSize { get; init; }
    public int BitstreamWidth { get; init; }
    public required SvStreamingConcatenationStreamExpression[] Streams { get; init; }
    public string? Kind { get; init; }
}
public record struct SvStreamingConcatenationStreamExpression(ISvExpression Operand, ISvExpression? WithExpr, string? ConstantWithWidth);

public record SvStringLiteral : ISvExpression
{
    public required string Type { get; init; }
    public string? Literal { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvTaggedUnion : ISvExpression
{
    public required string Type { get; init; }
    public required string Member { get; init; }
    public ISvExpression? ValueExpr { get; init; }
    public string? Kind { get; init; }
}
public record SvTimeLiteral : ISvExpression
{
    public required string Type { get; init; }
    public double Value { get; init; }
    public SvTimeScale? Const { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvTypeReference : ISvExpression
{
    public required string Type { get; init; }
    public string? TargetType { get; init; }
    public string? Kind { get; init; }
}
public record SvUnary : ISvExpression
{
    public required string Type { get; init; }
    public required ISvExpression Operand { get; init; }
    public required SvUnaryOperator Op { get; init; }
    public string? Kind { get; init; }
}
public record SvUnbasedUnsizedIntegerLiteral : ISvExpression
{
    public required string Type { get; init; }
    public required string Value { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; } 
}
public record SvUnboundedLiteral : ISvExpression
{
    public required string Type { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; } 
}
public record SvValueRange : ISvExpression
{
    public required string Type { get; init; }
    public ISvExpression? Left { get; init; }
    public ISvExpression? Right { get; init; }
    public SvValueRangeKind? RangeKind { get; init; }
    public string? Kind { get; init; }
}
public record SvUnaryOp : ISvExpression
{
    public required string Type { get; init; }
    public SvUnaryOperator? Op { get; init; }
    public ISvExpression? Operand { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}


public record SvConditionalOp : ISvExpression
{
    public required string Type { get; init; }
    public SvConditionItem[]? Conditions { get; init; }
    public ISvExpression? Left { get; init; }
    public ISvExpression? Right { get; init; }
    public string? Constant { get; init; }
    public string? Kind { get; init; }
    public SvAttribute[]? Attributes { get; init; }
}
public record SvConditionItem(ISvExpression Expr, ISvPattern Pattern);