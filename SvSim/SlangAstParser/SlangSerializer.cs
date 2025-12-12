using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using SvSim.SlangAstParser.Ast;

namespace SvSim.SlangAstParser;

public static class SlangSerializer
{
    public static JsonSerializerOptions GetOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { AutoRegisterAstNodes }
            }
        };
        return options;
    }
    private static readonly string[] Suffixes = 
    [
        "AssignmentPatternExpression", "BinsSelectExpr", "AssertionExpr", 
        "ValueSymbol", "Expression", "Constraint", "Statement", 
        "Control", "Pattern", "Symbol", "Type" 
    ];
    private static void AutoRegisterAstNodes(JsonTypeInfo typeInfo)
    {
        if (!typeInfo.Type.IsAbstract || !typeof(AstNode).IsAssignableFrom(typeInfo.Type)) return;
        
        typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "kind",
                
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
            IgnoreUnrecognizedTypeDiscriminators = true
        };

        var allTypes = Assembly.GetExecutingAssembly().GetTypes();

        var derivedTypes = allTypes
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeInfo.Type.IsAssignableFrom(t));

        foreach (var type in derivedTypes)
        {
            var name = type.Name;
            var suffix = Suffixes
                .OrderByDescending(s => s.Length)
                .FirstOrDefault(s => name.EndsWith(s));
            if (suffix is not null)
            {
                name = name[..^suffix.Length];
            }
            typeInfo.PolymorphismOptions.DerivedTypes.Add(
                new JsonDerivedType(type, name)
            );
        }
    }
}