using SvSim.SlangAstParser.Ast.Symbols.Types;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TypeReferenceExpression : Expression
{
    public SvType? TargetType;
};