using SvSim.SlangAstParser.Ast.Symbols;
using SvSim.SlangAstParser.Ast.Symbols.Types;

namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public record StructuredAssignmentPatternExpression : AssignmentPatternExpressionBase
{
    public IndexSetter[] IndexSetter = [];
    public MemberSetter[] MemberSetter = [];
    public TypeSetter[] TypeSetter = [];
    public Expression? DefaultSetter;
};

public struct IndexSetter
{
    public required Expression Index;
    public required Expression Expr;
}

public struct MemberSetter
{
    public required SvSymbol Member;
    public required Expression Expr;
}

public struct TypeSetter
{
    public required SvType Type;
    public required Expression Expr;
}