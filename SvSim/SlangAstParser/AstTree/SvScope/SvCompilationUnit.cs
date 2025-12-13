using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree.SvScope;

public record SvCompilationUnit : IKind
{
    public required string Name { get; init; }
    [JsonPropertyName("addr")]public required long Address { get; init; }
    [JsonPropertyName("members")] public IKind[] Members { get; init; } = [];
};