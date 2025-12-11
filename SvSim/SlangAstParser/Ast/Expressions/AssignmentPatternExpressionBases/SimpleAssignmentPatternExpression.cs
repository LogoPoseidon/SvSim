namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public record SimpleAssignmentPatternExpression : AssignmentPatternExpressionBase
{
    public bool IsLValue = false;
};