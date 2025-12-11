namespace SvSim.SlangAstParser.Ast.Expressions;

public record NewClassExpression : Expression
{
    public Expression? ConstructorCall;
    public bool? IsSuperClass;
};