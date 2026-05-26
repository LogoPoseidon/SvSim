using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol;

namespace SvSim.SlangAstParser.AstTree.Constraint;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvSolveBefore), nameof(SvConstraintKind.SolveBefore))]
[JsonDerivedType(typeof(SvConstraintList), nameof(SvConstraintKind.List))]
[JsonDerivedType(typeof(SvImplication), nameof(SvConstraintKind.Implication))]
[JsonDerivedType(typeof(SvForeach), nameof(SvConstraintKind.Foreach))]
[JsonDerivedType(typeof(SvDisableSoft), nameof(SvConstraintKind.DisableSoft))]
[JsonDerivedType(typeof(SvUniqueness), nameof(SvConstraintKind.Uniqueness))]
[JsonDerivedType(typeof(SvConditional), nameof(SvConstraintKind.Conditional))]
[JsonDerivedType(typeof(SvExpression), nameof(SvConstraintKind.Expression))]
[JsonDerivedType(typeof(SvInvalid), nameof(SvConstraintKind.Invalid))]
public interface ISvConstraint : ISvAstNode { }
public record SvConditional : ISvConstraint
{
    public required ISvExpression Predicate { get; init; }
    public required ISvConstraint IfBody { get; init; }
    public ISvConstraint? ElseBody { get; init; }
    public string? Kind { get; init; }
}
public record SvConstraintList : ISvConstraint
{
    public required ISvConstraint[] List { get; init; }
    public string? Kind { get; init; }
}
public record SvDisableSoft : ISvConstraint
{
    public required ISvExpression Target { get; init; }
    public string? Kind { get; init; }
}
public record SvExpression : ISvConstraint
{
    public required ISvExpression Expr { get; init; }
    public bool IsSoft { get; init; }
    public string? Kind { get; init; }
}
public record SvForeach : ISvConstraint
{
    public required ISvExpression ArrayRef { get; init; }
    public required LoopDim[] LoopDims { get; init; }
    public required ISvConstraint Body { get; init; }
    public string? Kind { get; init; }
}
public record LoopDim(string Range, ISvSymbol Var);

public record SvImplication : ISvConstraint
{
    public required ISvExpression Predicate { get; init; }
    public required ISvConstraint Body { get; init; }
    public string? Kind { get; init; }
}
public record SvInvalid : ISvConstraint
{
    public ISvConstraint? Child { get; init; }
    public string? Kind { get; init; }
}

public record SvSolveBefore : ISvConstraint
{
    public required ISvExpression[] Solve { get; init; }
    public required ISvExpression[] After { get; init; }
    public string? Kind { get; init; }
}
public record SvUniqueness : ISvConstraint
{
    public required ISvExpression[] Items { get; init; }
    public string? Kind { get; init; }
}