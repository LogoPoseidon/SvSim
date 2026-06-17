using System.Text.Json;
using System.Text.Json.Serialization;
using SvAstParser.AstTree.SvEnums;

namespace SvAstParser.Serializer;

internal static class SlangSerializer
{
    private static readonly JsonSerializerOptions ReflectionOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowOutOfOrderMetadataProperties = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters =
        {
            new JsonStringEnumConverter<SvVariableFlags>(JsonNamingPolicy.SnakeCaseLower),
            new JsonStringEnumConverter(),
            new SvInstanceBodyConverter(),
            new PooledStringConverter()
        }
    };

    internal static TopLevel Parse(ReadOnlySpan<byte> utf8JsonBytes)
    {
        var topLevel = JsonSerializer.Deserialize<TopLevel>(utf8JsonBytes, ReflectionOptions) ??
                       throw new JsonException("Failed to deserialize top level or no module given");

        SlangAstResolver.Resolve(topLevel);
        return topLevel;
    }
}