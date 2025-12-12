using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record UniquenessConstraint : SvConstraint
{
    public SvExpression[] Items = [];
};