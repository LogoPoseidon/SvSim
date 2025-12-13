using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree.SourceLocations;

public class SourceRange : ISourceLocation
{
    [JsonPropertyName("source_file_begin")] public required string FileBegin { get; set; }
    [JsonPropertyName("source_file_end")] public required string FileEnd { get; set; }
    [JsonPropertyName("source_line_begin")] public required int LineBegin { get; set; }
    [JsonPropertyName("source_line_end")] public required int LineEnd { get; set; }
    [JsonPropertyName("source_column_begin")] public required int ColBegin { get; set; }
    [JsonPropertyName("source_column_end")] public required int ColEnd { get; set; }
}