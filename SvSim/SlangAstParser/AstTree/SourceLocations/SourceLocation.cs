using System.Text.Json;
using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.AstTree.SourceLocations;

public abstract record SourceLocation : IJsonOnDeserialized
{
    [JsonIgnore] public ISourceLocation? Location { get; private set; }
    [JsonExtensionData] public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    public void OnDeserialized()
    {
        if (ExtensionData == null) return;

        if (ExtensionData.TryGetValue("source_file", out var file))
        {
            Location = new SourcePoint
            {
                File = file.GetString()!, // Cannot be null
                Line = GetInt(ExtensionData, "source_line"),
                Column = GetInt(ExtensionData, "source_column")
            };
        }
        else if (ExtensionData.TryGetValue("source_file_begin", out var fileBegin))
        {
            Location = new SourceRange
            {
                FileBegin = fileBegin.GetString()!, // Cannot be null
                FileEnd = GetString(ExtensionData, "source_file_end"),
                LineBegin = GetInt(ExtensionData, "source_line_begin"),
                LineEnd = GetInt(ExtensionData, "source_line_end"),
                ColBegin = GetInt(ExtensionData, "source_column_begin"),
                ColEnd = GetInt(ExtensionData, "source_column_end")
            };
        }
    }

    private static int GetInt(Dictionary<string, JsonElement> dict, string key) =>
        dict.TryGetValue(key, out var val) && val.ValueKind == JsonValueKind.Number ? val.GetInt32() : 0;

    private static string GetString(Dictionary<string, JsonElement> dict, string key) =>
        dict.TryGetValue(key, out var val) ? val.ToString() : "n/a";
}