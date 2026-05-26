using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.AstTree.TimingControl;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvDelay), nameof(SvTimingControlKind.Delay))]
[JsonDerivedType(typeof(SvOneStepDelay), nameof(SvTimingControlKind.OneStepDelay))]
[JsonDerivedType(typeof(SvEventList), nameof(SvTimingControlKind.EventList))]
[JsonDerivedType(typeof(SvBlockEventList), nameof(SvTimingControlKind.BlockEventList))]
[JsonDerivedType(typeof(SvSignalEvent), nameof(SvTimingControlKind.SignalEvent))]
[JsonDerivedType(typeof(SvCycleDelay), nameof(SvTimingControlKind.CycleDelay))]
[JsonDerivedType(typeof(SvRepeatedEvent), nameof(SvTimingControlKind.RepeatedEvent))]
[JsonDerivedType(typeof(SvImplicitEvent), nameof(SvTimingControlKind.ImplicitEvent))]
[JsonDerivedType(typeof(SvDelay3), nameof(SvTimingControlKind.Delay3))]
[JsonDerivedType(typeof(SvInvalidTiming), nameof(SvTimingControlKind.Invalid))]
public interface ISvTimingControl : ISvAstNode { }
public record SvBlockEventList : ISvTimingControl
{
    public ISvExpression[]? Events { get; init; }
    public string? Kind { get; init; }
}
public record SvCycleDelay : ISvTimingControl
{
    public required ISvExpression Expr { get; init; }
    public string? Kind { get; init; }
}
public record SvDelay3 : ISvTimingControl
{
    public required ISvExpression Expr1 { get; init; }
    public ISvExpression? Expr2 { get; init; }
    public ISvExpression? Expr3 { get; init; }

    public string? Kind { get; init; }
}
public record SvDelay : ISvTimingControl
{
    public required ISvExpression Expr { get; init; }
    public string? Kind { get; init; }
}
public record SvEventList : ISvTimingControl
{
    public ISvTimingControl[]? Events { get; init; }
    public string? Kind { get; init; }
}
public record SvImplicitEvent : ISvTimingControl
{
    public string? Kind { get; init; }
}

public record SvInvalidTiming : ISvTimingControl
{
    public string? Kind { get; init; }
    public required ISvTimingControl Child { get; init; }
}

public record SvOneStepDelay : ISvTimingControl
{
    public string? Kind { get; init; }
}

public record SvRepeatedEvent : ISvTimingControl
{
    public required ISvExpression Expr { get; init; }
    public required ISvTimingControl Event { get; init; }
    public string? Kind { get; init; }
}

public record SvSignalEvent : ISvTimingControl
{
    public required ISvExpression Expr { get; init; }
    public ISvExpression? Iff { get; init; }
    public SvEdgeKind? Edge { get; init; }
    public string? Kind { get; init; }
}
