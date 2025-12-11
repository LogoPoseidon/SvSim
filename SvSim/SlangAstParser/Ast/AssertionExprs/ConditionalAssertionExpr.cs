using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record ConditionalAssertionExpr : AssertionExpr
{
    public SvExpression? Condition;
    public required AssertionExpr IfExpr;
    public AssertionExpr? ElseExpr;
};