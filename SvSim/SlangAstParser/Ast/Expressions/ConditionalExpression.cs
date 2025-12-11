using SvSim.SlangAstParser.Ast.Patterns;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConditionalExpression : Expression
{
    public Expression? Left;
    public Expression? Right;
    public Expression? KnownSide;
    public Condition[] Conditions = [];
};

public struct Condition
{
    public required Expression Expr;
    public Pattern Pattern;
}