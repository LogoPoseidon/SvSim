using System.Text.Json;
using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol;
using SvSim.SlangAstParser.Serializer;

namespace SvSim.SlangAstParser.Tests;

[TestFixture]
public class AstIntegrationTests
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

    private static string[] GetAstJsonFiles()
    {
        var testDataDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
        return !Directory.Exists(testDataDir) ? [] : Directory.GetFiles(testDataDir, "*.json");
    }

    [TestCaseSource(nameof(GetAstJsonFiles))]
    public void Test_Deserialization_Succeeds(string jsonFilePath)
    {
        var jsonContent = File.ReadAllText(jsonFilePath);

        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            Assert.Ignore($"File is empty: {Path.GetFileName(jsonFilePath)}");
        }

        Assert.DoesNotThrow(() =>
            {
                var result = JsonSerializer.Deserialize<TopLevel>(jsonContent, SerializerOptions);

                Assert.That(result, Is.Not.Null, "Deserialization returned null.");
            }, $"Failed to parse JSON file: {Path.GetFileName(jsonFilePath)}");
    }
}