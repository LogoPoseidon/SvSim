namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConcatenationExpression : SvExpression
{
    public SvExpression[] Operands = [];
};