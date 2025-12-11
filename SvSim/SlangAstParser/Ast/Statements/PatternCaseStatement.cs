using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Ast.Patterns;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public record PatternCaseStatement : SvStatement
{
    public SvExpression? Expr;
    public ItemGroup[] Items = [];
    public SvStatement? DefaultCase;
    public CaseStatementCondition? Condition;
    public UniquePriorityCheck? Check;
    public struct ItemGroup
    {
        public required Pattern Pattern;
        public SvExpression? Filter;
        public required SvStatement Stmt;
    }
};

