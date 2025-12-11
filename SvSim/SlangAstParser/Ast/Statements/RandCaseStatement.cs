using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record RandCaseStatement : SvStatement
{
    public Item[] Items = [];
    public struct Item
    {
        public required SvExpression Expr;
        public required SvStatement Stmt;
    }
};