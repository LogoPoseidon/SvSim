using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.AssertionExprs;

public record CaseAssertionExpr : AssertionExpr
{
    public SvExpression? Expr;
    public ItemGroup[] Items = [];
    public AssertionExpr? DefaultCase;
    public struct ItemGroup
    {
        public SvExpression[] Expressions;
        public required AssertionExpr Body;
    }
};

