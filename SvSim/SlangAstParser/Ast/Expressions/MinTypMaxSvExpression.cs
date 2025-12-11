namespace SvSim.SlangAstParser.Ast.Expressions;

public record MinTypMaxSvExpression : SvExpression
{
    public SvExpression? Min;
    public SvExpression? Typ;
    public SvExpression? Max;
    public SvExpression? Selected;
};