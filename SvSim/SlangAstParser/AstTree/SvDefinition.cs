using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SourceLocations;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.AstTree;

public record SvDefinition : SourceLocation, IKind
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("addr")] public required long Address { get; init; }
    [JsonPropertyName("defaultNetType")] public required string DefaultNetType { get; init; }
    [JsonPropertyName("definitionKind")] public required string DefinitionKind { get; init; }
    [JsonPropertyName("defaultLifetime")]  public required string DefaultLifetime { get; init; }
    [JsonPropertyName("cellDefine")] public required bool CellDefine { get; init; }
    [JsonPropertyName("timeScale")] public required string TimeScale { get; init; }
    [JsonPropertyName("unconnectedDrive")] public required SvUnconnectedDrive UnconnectedDrive { get; init; }
}