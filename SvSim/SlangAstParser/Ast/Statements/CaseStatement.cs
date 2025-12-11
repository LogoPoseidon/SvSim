using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public record CaseStatement : SvStatement
{
    public SvExpression? Expr;
    public ItemGroup[] Items = [];
    public SvStatement? DefaultCase;
    public CaseStatementCondition? Condition;
    public UniquePriorityCheck? Check;
    public struct ItemGroup
    {
        public SvExpression[] Expressions;
        public required SvStatement Stmt;
    }
};

