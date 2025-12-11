namespace SvSim.SlangAstParser.Ast.Expressions.AssignmentPatternExpressionBases;

public abstract record AssignmentPatternSvExpressionBase : SvExpression
{
    public SvExpression[] Elements = [];
};