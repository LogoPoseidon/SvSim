namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public abstract record AssignmentPatternExpressionBase : Expression
{
    public Expression[] Elements = [];
};