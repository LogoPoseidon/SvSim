namespace SvSim.SlangAstParser.Ast.BinsSelectExprs;

public record UnaryBinsSelectExpr : BinsSelectExpr
{
    public Op Op;
};

public enum Op
{
    Negation
}