namespace SvSim.SlangAstParser.Ast.Constraints;

public record InvalidConstraint : SvConstraint
{
    public SvConstraint? Child;
};