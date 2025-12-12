namespace SvSim.SlangAstParser.Ast;

public record AstNode
{
    public string? SourceFileStart;
    public string? SourceFileEnd;
    public int? SourceLineStart;
    public int? SourceLineEnd;
    public int? SourceColumnStart;
    public int? SourceColumnEnd;
    public int? Addr;
    public AstNode[] Members = [];
    public string? Name;
};