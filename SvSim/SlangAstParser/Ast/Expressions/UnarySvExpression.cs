using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record UnarySvExpression : SvExpression
{
    public SvExpression? Operand;
    public UnaryOperator? Op;
};