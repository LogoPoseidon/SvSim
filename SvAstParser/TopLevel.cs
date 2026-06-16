using System.Text.Json.Serialization;
using SvAstParser.AstTree.Scope;
using SvAstParser.AstTree.Symbol;

namespace SvAstParser;

public record TopLevel
{
    [JsonPropertyName("design")]public required ISvScope Design { get; init; }
    [JsonPropertyName("definitions")]public required ISvSymbol[] Definitions { get; init; }
};