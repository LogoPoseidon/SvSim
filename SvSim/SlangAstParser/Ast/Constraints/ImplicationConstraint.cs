using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record ImplicationConstraint : SvConstraint
{
    public SvExpression? Predicate;
    public SvConstraint? Body;
}