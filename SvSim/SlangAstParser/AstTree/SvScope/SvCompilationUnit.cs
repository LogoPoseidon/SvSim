namespace SvSim.SlangAstParser.AstTree.SvScope;

public record SvCompilationUnit : IKind
{
    public required string Name { get; init; }
    public required long Address { get; init; }
    public required IKind[] Members { get; init; }
};