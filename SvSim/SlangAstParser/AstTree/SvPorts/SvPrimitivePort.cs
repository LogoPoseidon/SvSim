using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SourceLocations;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.AstTree.SvPorts;

public record SvPrimitivePort : SourceLocation, IKind
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Address { get; init; }
    [JsonPropertyName("type")] public required IType Type { get; init; }
    [JsonPropertyName("direction")] public required SvDirection Direction { get; init; }
};