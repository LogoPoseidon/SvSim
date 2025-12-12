using SvSim.SlangAstParser.Ast.Constraints;
using SvSim.SlangAstParser.Ast.Scopes;
using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols;
using SvSim.SlangAstParser.Ast.SystemSubroutines;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record CallExpression : SvExpression
{
    public SvExpression[] Arguments = [];
    public bool? IsSystemCall;
    public string? SubroutineName;
    public SubroutineKind? SubroutineKind;
    public bool? HasOutputArgs;
    public Func<object>? Subroutine;
};

public interface IExtraInfo;
public struct IteratorCallInfo : IExtraInfo
{
    public SvExpression? IterExpr;
    public ValueSymbol IterVar;
}

public struct RandomizeCallInfo : IExtraInfo
{
    public SvConstraint? InlineConstraints;
    public string[]? ConstraintRestrictions;
}

public struct SystemCallInfo
{
    public (SvExpression, ValueSymbol) IteratorInfo;
    public required SystemSubroutine Subroutine;
    public required SvScope SvScope;
    public IExtraInfo? ExtraInfo;
}