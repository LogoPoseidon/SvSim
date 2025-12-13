using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SourceLocations;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.SvType;

namespace SvSim.SlangAstParser.AstTree.SvPorts;

public record SvPrimitivePort : SourceLocation, IKind
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Address { get; init; }
    [JsonPropertyName("type")] public required SvType.SvType Type { get; init; }
    [JsonPropertyName("direction")] public required SvPrimitiveDirection Direction { get; init; }
};