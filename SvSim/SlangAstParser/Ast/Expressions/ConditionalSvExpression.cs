using SvSim.SlangAstParser.Ast.Patterns;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConditionalSvExpression : SvExpression
{
    public SvExpression? Left;
    public SvExpression? Right;
    public SvExpression? KnownSide;
    public Condition[] Conditions = [];
};

public struct Condition
{
    public required SvExpression Expr;
    public Pattern Pattern;
}