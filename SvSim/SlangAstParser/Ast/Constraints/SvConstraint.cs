using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Constraints;

public abstract record SvConstraint : AstNode
{
    public ConstraintKind? Kind;
};