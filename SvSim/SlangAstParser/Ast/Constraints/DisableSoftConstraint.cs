using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record DisableSoftConstraint : SvConstraint
{
    public SvExpression? Target;
};