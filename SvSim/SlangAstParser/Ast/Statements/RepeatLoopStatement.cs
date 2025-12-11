using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record RepeatLoopStatement : SvStatement
{
    public SvExpression? Count;
    public SvStatement? Body;
};