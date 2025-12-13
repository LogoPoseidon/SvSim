using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree;

public interface IKind
{
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("addr")] public long Address { get; init; }
}