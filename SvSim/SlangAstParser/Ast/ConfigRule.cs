namespace SvSim.SlangAstParser.Ast;

public record ConfigRule : AstNode
{
    public CellId? UseCell;
    public bool? IsUsed;
};

public struct CellId
{
    public string? Lib;
    public string? Name;
    public bool? TargetConfig;
}