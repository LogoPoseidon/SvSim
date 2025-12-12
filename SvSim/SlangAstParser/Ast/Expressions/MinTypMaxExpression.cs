namespace SvSim.SlangAstParser.Ast.Expressions;

public record MinTypMaxExpression : SvExpression
{
    public SvExpression? Min;
    public SvExpression? Typ;
    public SvExpression? Max;
    public SvExpression? Selected;
};