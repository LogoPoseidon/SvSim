namespace SvSim.SlangAstParser.Ast.Expressions;

public record NewClassExpression : SvExpression
{
    public SvExpression? ConstructorCall;
    public bool? IsSuperClass;
};