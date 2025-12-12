using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record ConditionalConstraint : SvConstraint
{
    public SvExpression? Predicate;
    public SvConstraint? IfBody;
    public SvConstraint? ElseBody;
};