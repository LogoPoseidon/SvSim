using System.Text.Json.Serialization;
using SvAstParser.AstTree.Expression;
using SvAstParser.AstTree.SvEnums;
using SvAstParser.AstTree.TimingControl;

namespace SvAstParser.AstTree.AssertionExpr;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvSequenceConcat), nameof(SvAssertionExprKind.SequenceConcat))]
[JsonDerivedType(typeof(SvSequenceWithMatch), nameof(SvAssertionExprKind.SequenceWithMatch))]
[JsonDerivedType(typeof(SvFirstMatch), nameof(SvAssertionExprKind.FirstMatch))]
[JsonDerivedType(typeof(SvStrongWeak), nameof(SvAssertionExprKind.StrongWeak))]
[JsonDerivedType(typeof(SvAbort), nameof(SvAssertionExprKind.Abort))]
[JsonDerivedType(typeof(SvSimple), nameof(SvAssertionExprKind.Simple))]
[JsonDerivedType(typeof(SvBinary), nameof(SvAssertionExprKind.Binary))]
[JsonDerivedType(typeof(SvUnary), nameof(SvAssertionExprKind.Unary))]
[JsonDerivedType(typeof(SvClocking), nameof(SvAssertionExprKind.Clocking))]
[JsonDerivedType(typeof(SvConditional), nameof(SvAssertionExprKind.Conditional))]
[JsonDerivedType(typeof(SvCase), nameof(SvAssertionExprKind.Case))]
[JsonDerivedType(typeof(SvDisableIff), nameof(SvAssertionExprKind.DisableIff))]
[JsonDerivedType(typeof(SvInvalid), nameof(SvAssertionExprKind.Invalid))]


public interface ISvAssertionExpr : ISvAstNode;

public record SvAbort : ISvAssertionExpr
{
    public ISvExpression? Expr { get; init; }
    public bool IsSync { get; init; }
    public SvAbortAssertionExpressionAction Action { get; init; }
    public string? Kind { get; init; }
}
public enum SvAbortAssertionExpressionAction{Accept, Reject}
public record SvBinary : ISvAssertionExpr
{
    public SvBinaryAssertionOperator Op { get; init; }
    public ISvAssertionExpr? Left { get; init; }
    public ISvAssertionExpr? Right { get; init; }
    public string? Kind { get; init; }
}
public record SvCase : ISvAssertionExpr
{
    public required ISvExpression Expr { get; init; }
    public required CaseItem[] Items { get; init; }
    public string? DefaultCase { get; init; }
    public string? Kind { get; init; }
}
public record CaseItem(string[] Expressions, ISvAssertionExpr Body);

public record SvClocking : ISvAssertionExpr
{
    public required ISvTimingControl Clocking { get; init; }
    public required ISvAssertionExpr Expr { get; init; }
    public string? Kind { get; init; }
}
public record SvConditional : ISvAssertionExpr
{
    public required ISvExpression Conditions { get; init; }
    public required ISvAssertionExpr IfTrue { get; init; }
    public string? ElseExpr { get; init; }
    public string? Kind { get; init; }
}
public record SvDisable : ISvAssertionExpr
{
    public required ISvExpression Condition { get; init; }
    public required ISvAssertionExpr Expr { get; init; }
    public string? Kind { get; init; }
}
public record SvFirstMatch : ISvAssertionExpr
{
    public ISvAssertionExpr? Seq { get; init; }
    public required ISvExpression[] MatchItems { get; init; }
    public string? Kind { get; init; }
}
public record SvInvalid : ISvAssertionExpr
{
    public required ISvAssertionExpr Child { get; init; }
    public string? Kind { get; init; }
}

public record SvSequenceConcat : ISvAssertionExpr
{
    public SequenceConcatElement[]? Elements { get; init; }
    public string? Kind { get; init; }
}
public record SequenceConcatElement(ISvAssertionExpr Sequence, int Min, int? Max);

public record SvSequenceWithMatch : ISvAssertionExpr
{
    public required ISvAssertionExpr Expr { get; init; }
    public required ISvExpression[] MatchItems { get; init; }
    public SequenceRepetition? Repetition { get; init; }
    public string? Kind { get; init; }
}

public record SequenceRepetition
{
    public required SequenceRepetitionKind Kind { get; init; }
    public int Min { get; init; }
    public int Max { get; init; }
}

public enum SequenceRepetitionKind {Consecutive, NonConsecutive, GoTo}

public record SvSimple : ISvAssertionExpr
{
    public required ISvExpression Expr { get; init; }
    public SequenceRepetition? Repetition { get; init; }
    public bool IsNullExpr { get; init; }
    public string? Kind { get; init; }
}
public record SvStrongWeak : ISvAssertionExpr
{
    public required ISvAssertionExpr Expr { get; init; }
    public StrongWeakAssertionStrength Strength { get; init; }
    public string? Kind { get; init; }
}
public enum StrongWeakAssertionStrength { Strong, Weak}

public record SvUnary : ISvAssertionExpr
{
    public required ISvAssertionExpr Expr { get; init; }
    public SvUnaryAssertionOperator Op { get; init; }
    public SvSequenceRange? Range { get; init; }
    public string? Kind { get; init; }
}

public record SvSequenceRange (uint Min, uint? Max);
public record SvDisableIff : ISvAssertionExpr
{
    public required ISvExpression Condition { get; init; }
    public required ISvAssertionExpr Expr { get; init; }
    public string? Kind { get; init; }
}