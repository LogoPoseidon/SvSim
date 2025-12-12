using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record UnaryExpression : SvExpression
{
    public SvExpression? Operand;
    public UnaryOperator? Op;
};