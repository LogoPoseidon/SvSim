using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SourceLocations;
using SvSim.SlangAstParser.AstTree.SvPrimitiveTables;

namespace SvSim.SlangAstParser.AstTree;

public record SvPrimitive : SourceLocation, IKind
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Address { get; init; }
    [JsonPropertyName("members")] public required IKind[] Members { get; init; }
    [JsonPropertyName("isSequential")] public required bool IsSequential { get; init; }
    [JsonPropertyName("table")] public required SvPrimitiveTable[] SvPrimitiveTable { get; init; }
}
