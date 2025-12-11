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
            
            // This is where the magic happens
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { AutoRegisterAstNodes }
            }
        };
        return options;
    }
    private static void AutoRegisterAstNodes(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type.IsAbstract && typeof(AstNode).IsAssignableFrom(typeInfo.Type))
        {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "kind",
                
                // If we find a node we haven't coded yet, don't crash, just ignore it
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                IgnoreUnrecognizedTypeDiscriminators = true
            };

            // 1. Find all non-abstract classes in the assembly
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();

            // 2. Filter: Must inherit from the current abstract class (e.g., AstNode)
            var derivedTypes = allTypes
                .Where(t => t.IsClass && !t.IsAbstract && typeInfo.Type.IsAssignableFrom(t));

            // 3. Register them
            foreach (var type in derivedTypes)
            {
                // MAGIC: Use the Class Name as the JSON "kind" discriminator
                // If your class is "BinaryOp", it maps to JSON "kind": "BinaryOp"
                typeInfo.PolymorphismOptions.DerivedTypes.Add(
                    new JsonDerivedType(type, type.Name)
                );
            }
        }
    }
}