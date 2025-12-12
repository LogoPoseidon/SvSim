namespace SvSim.SlangAstParser.Ast.Expressions;

public record InsideExpression : SvExpression
{
    public SvExpression? Left;
    public SvExpression[] RangeList = [];
};