using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree;

namespace SvSim.SlangAstParser;

public record TopLevel
{
    [JsonPropertyName("design")]public required SvDesign Design { get; init; }
    [JsonPropertyName("definitions")]public required IKind[] Definitions { get; init; }
};