using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record UnaryExpression : Expression
{
    public Expression? Operand;
    public UnaryOperator? Op;
};