namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public record SimpleAssignmentPatternSvExpression : AssignmentPatternSvExpressionBase
{
    public bool IsLValue = false;
};