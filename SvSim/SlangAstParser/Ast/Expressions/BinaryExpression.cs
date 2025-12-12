using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record BinaryExpression : SvExpression
{
    public SvExpression? Left;
    public SvExpression? Right;
    public BinaryOperator? Op;
};