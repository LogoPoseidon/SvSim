using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record BinaryExpression : Expression
{
    public Expression? Left;
    public Expression? Right;
    public BinaryOperator? Op;
};