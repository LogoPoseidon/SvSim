using SvSim.SlangAstParser.Ast.Symbols;
using SvSim.SlangAstParser.Ast.Symbols.Types;

namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public record StructuredAssignmentPatternSvExpression : AssignmentPatternSvExpressionBase
{
    public IndexSetter[] IndexSetter = [];
    public MemberSetter[] MemberSetter = [];
    public TypeSetter[] TypeSetter = [];
    public SvExpression? DefaultSetter;
};

public struct IndexSetter
{
    public required SvExpression Index;
    public required SvExpression Expr;
}

public struct MemberSetter
{
    public required SvSymbol Member;
    public required SvExpression Expr;
}

public struct TypeSetter
{
    public required SvType Type;
    public required SvExpression Expr;
}