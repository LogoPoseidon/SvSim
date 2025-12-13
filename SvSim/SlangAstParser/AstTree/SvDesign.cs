using System.Text.Json;
using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree;

public record SvDesign: IKind
{
    [JsonExtensionData] public Dictionary<string, JsonElement>? Data { get; init; }
};