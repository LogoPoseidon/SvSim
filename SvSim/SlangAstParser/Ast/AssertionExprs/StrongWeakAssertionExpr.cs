namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record StrongWeakAssertionExpr : AssertionExpr
{
    public AssertionStrength Strength;
    public required AssertionExpr Expr;
};
public enum AssertionStrength
{
    Strong,
    Weak
}