namespace SvSim.SlangAstParser.Ast.Constraints;

public record ConstraintList : SvConstraint
{
    public SvConstraint[] List = [];
};