using SvSim.SlangAstParser.Ast.Symbols.Types;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TypeReferenceExpression : SvExpression
{
    public SvType? TargetType;
};