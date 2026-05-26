using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.AstTree.BinsSelectExpr;

using System.Collections.Generic;
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvCrossId), nameof(SvBinsSelectExprKind.CrossId))]
[JsonDerivedType(typeof(SvCondition), nameof(SvBinsSelectExprKind.Condition))]
[JsonDerivedType(typeof(SvBinary), nameof(SvBinsSelectExprKind.Binary))]
[JsonDerivedType(typeof(SvUnary), nameof(SvBinsSelectExprKind.Unary))]
[JsonDerivedType(typeof(SvBinSelectWithFilter), nameof(SvBinsSelectExprKind.WithFilter))]
[JsonDerivedType(typeof(SvSetExpr), nameof(SvBinsSelectExprKind.SetExpr))]
[JsonDerivedType(typeof(SvInvalid), nameof(SvBinsSelectExprKind.Invalid))]
public interface ISvBinsSelectExpr : ISvAstNode { }
public record SvCrossId : ISvBinsSelectExpr
{
    public required string Cross { get; init; }
    public string[]? Id { get; init; }
    public string? Kind { get; init; }
}
public record SvCondition : ISvBinsSelectExpr
{
    public required string Target { get; init; }
    public IReadOnlyList<ISvExpression>? Intersects { get; init; }
    public string? Kind { get; init; }
}
public record SvBinary : ISvBinsSelectExpr
{
    public required ISvBinsSelectExpr Left { get; init; }
    public required ISvBinsSelectExpr Right { get; init; }
    public required string Op { get; init; }
    public string? Kind { get; init; }
}
public record SvUnary : ISvBinsSelectExpr
{
    public required ISvBinsSelectExpr Expr { get; init; }
    public required UnaryBinsSelectExprOp Op { get; init; }
    public string? Kind { get; init; }
}

public enum UnaryBinsSelectExprOp
{
    Negation
}

public record SvBinSelectWithFilter : ISvBinsSelectExpr
{
    public required ISvBinsSelectExpr Expr { get; init; }
    public required ISvExpression Filter { get; init; }
    public string? MatchesExpr { get; init; }
    public string? Kind { get; init; }
}
public record SvSetExpr : ISvBinsSelectExpr
{
    public required ISvExpression Expr { get; init; }
    public string? Kind { get; init; }
}
public record SvInvalid : ISvBinsSelectExpr
{
    public string? Kind { get; init; }
}