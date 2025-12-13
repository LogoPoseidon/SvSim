using System.Text.Json;
using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser;

public static class SlangSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    public static T? Parse<T>(string json)
    {
        
        return JsonSerializer.Deserialize<T>(json, Options);
    }
    
}