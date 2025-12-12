using SvSim.SlangAstParser.Ast.Patterns;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConditionalExpression : SvExpression
{
    public SvExpression? Left;
    public SvExpression? Right;
    public SvExpression? KnownSide;
    public Condition[] Conditions = [];
};

public struct Condition
{
    public required SvExpression Expr;
    public SvPattern SvPattern;
}