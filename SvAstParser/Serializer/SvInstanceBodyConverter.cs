using System.Text.Json;
using System.Text.Json.Serialization;
using SvAstParser.AstTree.Symbol;

namespace SvAstParser.Serializer;

internal class SvInstanceBodyConverter : JsonConverter<SvInstanceBody>
{
    public override SvInstanceBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrEmpty(str)) return null!;

            var span = str.AsSpan();
            var spaceIdx = span.IndexOf(' ');

            long addr = 0;
            string name;

            if (spaceIdx != -1)
            {
                _ = long.TryParse(span[..spaceIdx], out addr);
                name = span[(spaceIdx + 1)..].ToString();
            }
            else
            {
                name = str;
            }

            return new SvInstanceBody
            {
                Name = name,
                Addr = addr,
                Definition = name,
                Kind = "InstanceBody"
            };
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject or String for SvInstanceBody.");
        }

        string? nameProp = null;
        long addrProp = 0;
        ISvSymbol[]? membersProp = null;
        string? definitionProp = null;
        string? kindProp = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            if (reader.ValueTextEquals("name"u8))
            {
                reader.Read();
                nameProp = reader.GetString();
            }
            else if (reader.ValueTextEquals("addr"u8))
            {
                reader.Read();
                addrProp = reader.GetInt64();
            }
            else if (reader.ValueTextEquals("definition"u8))
            {
                reader.Read();
                definitionProp = reader.GetString();
            }
            else if (reader.ValueTextEquals("kind"u8))
            {
                reader.Read();
                kindProp = reader.GetString();
            }
            else if (reader.ValueTextEquals("members"u8))
            {
                reader.Read();
                membersProp = JsonSerializer.Deserialize<ISvSymbol[]>(ref reader, options);
            }
            else
            {
                reader.Read();
                reader.Skip();
            }
        }

        return new SvInstanceBody
        {
            Name = nameProp ?? string.Empty,
            Addr = addrProp,
            Members = membersProp,
            Definition = definitionProp ?? string.Empty,
            Kind = kindProp
        };
    }

    public override void Write(Utf8JsonWriter writer, SvInstanceBody value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("name", value.Name);
        writer.WriteNumber("addr", value.Addr);
        writer.WriteString("definition", value.Definition);
        if (value.Kind != null) writer.WriteString("kind", value.Kind);

        if (value.Members != null)
        {
            writer.WritePropertyName("members");
            JsonSerializer.Serialize(writer, value.Members, options);
        }

        writer.WriteEndObject();
    }
}