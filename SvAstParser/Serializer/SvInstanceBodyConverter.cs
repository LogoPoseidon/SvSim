using System.Text.Json;
using System.Text.Json.Serialization;
using SvAstParser.AstTree.Symbol;

namespace SvAstParser.Serializer;

internal record SvInstanceBodyDto(string Name, long Addr, ISvSymbol[]? Members, string Definition, string? Kind);

internal class SvInstanceBodyConverter : JsonConverter<SvInstanceBody>
{
    public override SvInstanceBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString()!;
            var parts = str.Split(' ', 2);
            var addr = parts.Length > 0 && long.TryParse(parts[0], out var parsedAddr) ? parsedAddr : 0;
            var name = parts.Length > 1 ? parts[1] : str;

            return new SvInstanceBody 
            { 
                Name = name, 
                Addr = addr, 
                Definition = name, 
                Kind = "InstanceBody" 
            };
        }

        var dto = JsonSerializer.Deserialize<SvInstanceBodyDto>(ref reader, options);
        if (dto is null) return null!;

        return new SvInstanceBody
        {
            Name = dto.Name,
            Addr = dto.Addr,
            Members = dto.Members,
            Definition = dto.Definition,
            Kind = dto.Kind
        };
    }

    public override void Write(Utf8JsonWriter writer, SvInstanceBody value, JsonSerializerOptions options)
    {
        var dto = new SvInstanceBodyDto(value.Name, value.Addr, value.Members, value.Definition, value.Kind);
        JsonSerializer.Serialize(writer, dto, options);
    }
}