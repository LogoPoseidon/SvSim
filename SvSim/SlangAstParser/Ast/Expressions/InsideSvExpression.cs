namespace SvSim.SlangAstParser.Ast.Expressions;

public record InsideSvExpression : SvExpression
{
    public SvExpression? Left;
    public SvExpression[] RangeList = [];
};