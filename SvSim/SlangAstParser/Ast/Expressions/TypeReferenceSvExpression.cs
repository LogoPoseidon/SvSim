using SvSim.SlangAstParser.Ast.Symbols.Types;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record TypeReferenceSvExpression : SvExpression
{
    public SvType? TargetType;
};