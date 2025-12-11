using SvSim.SlangAstParser.Ast.Expressions;

namespace SvSim.SlangAstParser.Ast.Statements;

public record WaitOrderStatement : SvStatement
{
    public SvExpression[] Events = [];
    public SvStatement? IfTrue;
    public SvStatement? IfFalse;
};