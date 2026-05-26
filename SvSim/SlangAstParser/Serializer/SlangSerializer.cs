using System.Text.Json;
using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.Serializer
{
    public static class SlangSerializer
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowOutOfOrderMetadataProperties = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            Converters =
            {
                new JsonStringEnumConverter<SvVariableFlags>(JsonNamingPolicy.SnakeCaseLower),
                new JsonStringEnumConverter(),
                new SvInstanceBodyConverter()
            }
        };

        public static TopLevel Parse(string json)
        {
            var topLevel = JsonSerializer.Deserialize<TopLevel>(json, SerializerOptions) ??
                           throw new JsonException("Failed to deserialize top level or no module given");

            SlangAstResolver.Resolve(topLevel);

            return topLevel;
        }
    }
}