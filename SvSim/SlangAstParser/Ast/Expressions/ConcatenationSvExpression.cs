namespace SvSim.SlangAstParser.Ast.Expressions;

public record ConcatenationSvExpression : SvExpression
{
    public SvExpression[] Operands = [];
};