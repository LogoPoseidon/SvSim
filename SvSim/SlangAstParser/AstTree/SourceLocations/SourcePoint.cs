using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree.SourceLocations;

public record SourcePoint : ISourceLocation
{
    [JsonPropertyName("source_file")] public required string File { get; set; }
    [JsonPropertyName("source_line")] public required int Line { get; set; }
    [JsonPropertyName("source_column")] public required int Column { get; set; }
}