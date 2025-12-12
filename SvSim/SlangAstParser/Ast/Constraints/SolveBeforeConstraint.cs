using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Constraints;

public record SolveBeforeConstraint : SvConstraint
{
    public SvExpression[] Solve = [];
    public SvExpression[] After = [];
};