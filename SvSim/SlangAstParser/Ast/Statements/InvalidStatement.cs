namespace SvSim.SlangAstParser.Ast.Statements;

public record InvalidStatement : SvStatement
{
    public SvStatement? Child;
}