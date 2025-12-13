using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.AstTree.SvType;

public record SvType
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Addr { get; init; }
    [JsonPropertyName("kind")] public required SvTypeKind Kind { get; init; }
};