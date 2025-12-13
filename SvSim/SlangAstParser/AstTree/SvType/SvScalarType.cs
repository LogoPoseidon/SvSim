using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree.SvType;

public record SvScalarType : IType
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Addr { get; init; }
};