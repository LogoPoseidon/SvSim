using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.AssertionExpr;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.Pattern;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol.TempVarSymbol;
using SvSim.SlangAstParser.AstTree.TimingControl;

namespace SvSim.SlangAstParser.AstTree.Statement;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvExpressionStatement), nameof(SvStatementKind.ExpressionStatement))]
[JsonDerivedType(typeof(SvList), nameof(SvStatementKind.List))]
[JsonDerivedType(typeof(SvReturn), nameof(SvStatementKind.Return))]
[JsonDerivedType(typeof(SvBlock), nameof(SvStatementKind.Block))]
[JsonDerivedType(typeof(SvVariableDeclaration), nameof(SvStatementKind.VariableDeclaration))]
[JsonDerivedType(typeof(SvRepeatLoop), nameof(SvStatementKind.RepeatLoop))]
[JsonDerivedType(typeof(SvTimed), nameof(SvStatementKind.Timed))]
[JsonDerivedType(typeof(SvWaitFork), nameof(SvStatementKind.WaitFork))]
[JsonDerivedType(typeof(SvWaitOrder), nameof(SvStatementKind.WaitOrder))]
[JsonDerivedType(typeof(SvProceduralAssign), nameof(SvStatementKind.ProceduralAssign))]
[JsonDerivedType(typeof(SvProceduralDeassign), nameof(SvStatementKind.ProceduralDeassign))]
[JsonDerivedType(typeof(SvEmpty), nameof(SvStatementKind.Empty))]
[JsonDerivedType(typeof(SvForeverLoop), nameof(SvStatementKind.ForeverLoop))]
[JsonDerivedType(typeof(SvBreak), nameof(SvStatementKind.Break))]
[JsonDerivedType(typeof(SvContinue), nameof(SvStatementKind.Continue))]
[JsonDerivedType(typeof(SvWhileLoop), nameof(SvStatementKind.WhileLoop))]
[JsonDerivedType(typeof(SvForLoop), nameof(SvStatementKind.ForLoop))]
[JsonDerivedType(typeof(SvForeachLoop), nameof(SvStatementKind.ForeachLoop))]
[JsonDerivedType(typeof(SvDoWhileLoop), nameof(SvStatementKind.DoWhileLoop))]
[JsonDerivedType(typeof(SvDisableFork), nameof(SvStatementKind.DisableFork))]
[JsonDerivedType(typeof(SvProceduralChecker), nameof(SvStatementKind.ProceduralChecker))]
[JsonDerivedType(typeof(SvConditional), nameof(SvStatementKind.Conditional))]
[JsonDerivedType(typeof(SvCase), nameof(SvStatementKind.Case))]
[JsonDerivedType(typeof(SvPatternCase), nameof(SvStatementKind.PatternCase))]
[JsonDerivedType(typeof(SvWait), nameof(SvStatementKind.Wait))]
[JsonDerivedType(typeof(SvRandCase), nameof(SvStatementKind.RandCase))]
[JsonDerivedType(typeof(SvRandSequence), nameof(SvStatementKind.RandSequence))]
[JsonDerivedType(typeof(SvImmediateAssertion), nameof(SvStatementKind.ImmediateAssertion))]
[JsonDerivedType(typeof(SvConcurrentAssertion), nameof(SvStatementKind.ConcurrentAssertion))]
[JsonDerivedType(typeof(SvDisable), nameof(SvStatementKind.Disable))]
[JsonDerivedType(typeof(SvEventTrigger), nameof(SvStatementKind.EventTrigger))]
[JsonDerivedType(typeof(SvInvalid), nameof(SvStatementKind.Invalid))]
public interface ISvStatement : ISvAstNode;

public record SvCase : ISvStatement
{
    public required ISvExpression Expr { get; init; }
    public required CaseStatementItemGroup[] Items { get; init; }
    public ISvStatement? DefaultCase { get; init; }
    public SvCaseStatementCondition? Condition { get; init; }
    public SvUniquePriorityCheck Check { get; init; }
    public SvAttribute[]? Attributes { get; init; }
    public string? Kind { get; init; }
}

public record struct CaseStatementItemGroup(ISvExpression[] Expressions, ISvStatement Stmt);

public record SvConditional : ISvStatement
{
    public required SvConditionalCondition[] Conditions { get; init; }
    public required ISvStatement IfTrue { get; init; }
    public ISvStatement? IfFalse { get; init; }

    public SvUniquePriorityCheck Check { get; init; }
    // public ISvExpression? Predicate { get; init; }
    // public ISvStatement? IfBody { get; init; }
    // public ISvStatement? ElseBody { get; init; }
    // public ISvExpression? Condition { get; init; }
    // public IKind? If { get; init; }
    // public IKind? Else { get; init; }
    public string? Kind { get; init; }
}
public record SvConditionalCondition
{
    public required ISvExpression Expr { get; init; }
    public ISvPattern? Pattern { get; init; }
}

public record SvExpressionStatement : ISvStatement
{
    public ISvExpression? Expr { get; init; }
    public string? Kind { get; init; }
}

public record SvList : ISvStatement
{
    public required ISvStatement[] List { get; init; }
    public string? Kind { get; init; }
}

public record SvReturn : ISvStatement
{
    public ISvExpression? Expr { get; init; }
    public string? Kind { get; init; }
}

public record SvBlock : ISvStatement
{
    public required ISvStatement Body { get; init; }
    public string? Block { get; init; }
    public SvStatementBlockKind BlockKind { get; init; }
    public string? Kind { get; init; }
}

public record SvVariableDeclaration : ISvStatement
{
    public required string Symbol { get; init; }
    public string? Kind { get; init; }
}

public record SvRepeatLoop : ISvStatement
{
    public required ISvExpression Count { get; init; }
    public required ISvStatement Body { get; init; }
    public string? Kind { get; init; }
}

public record SvTimed : ISvStatement
{
    public required ISvTimingControl Timing { get; init; }
    public required ISvStatement Stmt { get; init; }
    public string? Kind { get; init; }
}

public record SvWaitFork : ISvStatement
{
    public string? Kind { get; init; }
}

public record SvWaitOrder : ISvStatement
{
    public required string[] Events { get; init; }
    public string? IfTrue { get; init; }
    public string? IfFalse { get; init; }

    public string? Kind { get; init; }
}

public record WaitOrderEvent(ISvExpression Target);

public record SvProceduralAssign : ISvStatement
{
    public ISvExpression? Assignment { get; init; }
    public bool IsForce { get; init; }
    public string? Kind { get; init; }
}

public record SvProceduralDeassign : ISvStatement
{
    public required ISvExpression Lvalue { get; init; }
    public bool IsRelease { get; init; }
    public string? Kind { get; init; }
}

public record SvEmpty : ISvStatement
{
    public string? Kind { get; init; }
}

public record SvEventTrigger : ISvStatement
{
    public required ISvExpression Target { get; init; }
    public string? Timing { get; init; }
    public bool IsNonBlocking { get; init; }
    public string? Kind { get; init; }
}

public record SvForeverLoop : ISvStatement
{
    public required ISvStatement Body { get; init; }
    public string? Kind { get; init; }
}

public record SvBreak : ISvStatement
{
    public string? Kind { get; init; }
}

public record SvContinue : ISvStatement
{
    public string? Kind { get; init; }
}

public record SvWhileLoop : ISvStatement
{
    public required ISvExpression Cond { get; init; }
    public required ISvStatement Body { get; init; }
    public string? Kind { get; init; }
}

public record SvForLoop : ISvStatement
{
    public ISvExpression[]? Initializers { get; init; }
    public string[]? LoopVars { get; init; }
    public ISvExpression? StopExpr { get; init; }
    public ISvExpression[]? Steps { get; init; }
    public ISvStatement? Body { get; init; }
    public string? Kind { get; init; }
}

public record SvForeachLoop : ISvStatement
{
    public required ISvExpression ArrayRef { get; init; }
    public required LoopDim[] LoopDims { get; init; }
    public required ISvStatement Body { get; init; }
    public string? Kind { get; init; }
}

public record LoopDim(string? Range, SvIterator Var);

public record SvDoWhileLoop : ISvStatement
{
    public ISvExpression? Cond { get; init; }
    public ISvStatement? Body { get; init; }
    public string? Kind { get; init; }
}

public record SvDisableFork : ISvStatement
{
    public string? Kind { get; init; }
}

public record SvDisable : ISvStatement
{
    public required ISvExpression Target { get; init; }
    public string? Kind { get; init; }
}

public record SvProceduralChecker : ISvStatement
{
    public required ISvSymbol[] Instances { get; init; }
    public string? Kind { get; init; }
}

public record ProceduralCheckerInstance(string Instance);

public record SvPatternCase : ISvStatement
{
    public required ISvExpression Expr { get; init; }
    public required CaseItem[] Items { get; init; }
    public ISvStatement? DefaultCase { get; init; }
    public SvCaseStatementCondition Condition { get; init; }
    public SvUniquePriorityCheck? Check { get; init; }
    public string? Kind { get; init; }
}

public record CaseItem(ISvPattern Pattern, ISvExpression? Filter, ISvStatement Stmt);

public record SvWait : ISvStatement
{
    public required ISvExpression Cond { get; init; }
    public required ISvStatement Stmt { get; init; }
    public string? Kind { get; init; }
}

public record SvRandCase : ISvStatement
{
    public required RandCaseItem[] Items { get; init; }
    public string? Kind { get; init; }
}
public record RandCaseItem(ISvExpression Expr, ISvStatement Stmt);

public record SvRandSequence : ISvStatement
{
    public required string FirstProduction { get; init; }
    public string? Kind { get; init; }
}

public record SvImmediateAssertion : ISvStatement
{
    public required ISvExpression Cond { get; init; }
    public ISvStatement? IfTrue { get; init; }
    public ISvStatement? IfFalse { get; init; }
    public SvAssertionKind AssertionKind { get; init; }
    public bool IsDeferred { get; init; }
    public bool IsFinal { get; init; }
    public string? Kind { get; init; }
}

public record SvConcurrentAssertion : ISvStatement
{
    public ISvAssertionExpr? PropertySpec { get; init; }
    public ISvStatement? IfTrue { get; init; }
    public ISvStatement? IfFalse { get; init; }
    public SvAssertionKind? AssertionKind { get; init; }
    public string? Kind { get; init; }
}
public record SvInvalid : ISvStatement
{
    public ISvStatement? Child { get; init; }
    public string? Kind { get; init; }
}