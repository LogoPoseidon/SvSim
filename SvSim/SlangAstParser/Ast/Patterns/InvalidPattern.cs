namespace SvSim.SlangAstParser.Ast.Patterns;

public record InvalidPattern : SvPattern
{
    public SvPattern? Child;
};