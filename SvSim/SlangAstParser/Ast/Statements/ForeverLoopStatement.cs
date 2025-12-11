namespace SvSim.SlangAstParser.Ast.Statements;

public record ForeverLoopStatement : SvStatement
{
    public SvStatement? Body;
};