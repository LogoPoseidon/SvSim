using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record WaitStatement : SvStatement
{
    public SvExpression? Cond;
    public SvStatement? Stmt;
};