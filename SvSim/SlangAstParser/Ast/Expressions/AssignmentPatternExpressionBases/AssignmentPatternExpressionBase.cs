namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public abstract record AssignmentPatternExpressionBase : SvExpression
{
    public SvExpression[] Elements = [];
};