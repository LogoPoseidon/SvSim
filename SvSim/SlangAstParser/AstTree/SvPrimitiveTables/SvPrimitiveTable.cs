using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree.SvPrimitiveTables;

public record SvPrimitiveTable
{
    [JsonPropertyName("inputs")] public required string Inputs { get; init; }
    [JsonPropertyName("output")] public required string Output { get; init; }
    [JsonPropertyName("state")] public string? States { get; init; }
};