namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConcatenationExpression : Expression
{
    public Expression[] Operands = [];
};