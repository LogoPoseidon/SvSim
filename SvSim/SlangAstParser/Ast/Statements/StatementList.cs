namespace SvSim.SlangAstParser.Ast.Statements;

public record StatementList : SvStatement
{
    public SvStatement[] List = [];
};