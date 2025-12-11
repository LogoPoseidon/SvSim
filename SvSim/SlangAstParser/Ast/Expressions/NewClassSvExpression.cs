namespace SvSim.SlangAstParser.Ast.Expressions;

public record NewClassSvExpression : SvExpression
{
    public SvExpression? ConstructorCall;
    public bool? IsSuperClass;
};