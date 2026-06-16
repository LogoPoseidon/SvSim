using System.Text.Json.Serialization;

namespace SvAstParser.AstTree.Expression.AssignmentPatternExpression;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvReplicatedAssignmentPattern), "ReplicatedAssignmentPattern")]
[JsonDerivedType(typeof(SvSimpleAssignmentPattern), "SimpleAssignmentPattern")]
[JsonDerivedType(typeof(SvStructuredAssignmentPattern), "StructuredAssignmentPattern")]

public interface ISvAssignmentPatternExpression : ISvExpression
{
    public ISvExpression[] Elements { get; init; }
}
public record SvReplicatedAssignmentPattern : ISvAssignmentPatternExpression
{
    public required string Type { get; init; }
    public ISvExpression? Count { get; init; }
    public required ISvExpression[] Elements { get; init; }
    public string? Kind { get; init; }
}
public record SvSimpleAssignmentPattern : ISvAssignmentPatternExpression
{
    public required string Type { get; init; }
    public required ISvExpression[] Elements { get; init; }
    public bool IsLValue { get; init; }
    public string? Kind { get; init; }
    public string? Constant { get; init; }
}
public record SvStructuredAssignmentPattern : ISvAssignmentPatternExpression
{
    public required string Type { get; init; }
    public ISvExpression[]? Elements { get; init; }
    public StructuredPatternMemberSetter[]? MemberSetters { get; init; }
    public StructuredPatternTypeSetter[]? TypeSetters { get; init; }
    public StructuredPatternIndexSetter[]? IndexSetters { get; init; }
    public ISvExpression? DefaultSetter { get; init; }
    public string? Kind { get; init; }
}

public record StructuredPatternMemberSetter(string? Member, ISvExpression? Expr);
public record StructuredPatternTypeSetter(string? Type, ISvExpression? Expr);
public record StructuredPatternIndexSetter(ISvExpression? Index, ISvExpression? Expr);
