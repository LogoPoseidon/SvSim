using System.Text.Json;
using System.Text.Json.Serialization;

namespace SvSim.SlangAstParser.Serializer
{
    public static class SlangSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowOutOfOrderMetadataProperties = true, 
            Converters =
            {
                new JsonStringEnumConverter(),
            }
        };
        public static TopLevel? Parse(string json)
        {
        
            return JsonSerializer.Deserialize<TopLevel>(json, Options);
        }
    
    }
}