using System.Text.Json;
using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree;

public record SvDesign: IKind
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Address { get; init; }
    [JsonPropertyName("members")] public required IKind[] Members { get; init; }
};