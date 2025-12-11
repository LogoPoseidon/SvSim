using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record DoWhileLoopStatement : SvStatement
{
    public SvExpression? Cond;
    public SvStatement? Body;
};