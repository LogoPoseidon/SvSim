using SvSim.SlangAstParser.Ast.Symbols.Types;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public abstract record SvExpression : AstNode
{
    public required EffectiveSign EffectiveSign;
    public required ExpressionKind Kind;
    public required SvType Type;
};

public enum EffectiveSign
{
    Unsigned,
    Signed,
    Either
}