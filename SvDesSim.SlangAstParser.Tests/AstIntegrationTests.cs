using SvAstParser;

namespace SvSim.SlangAstParser.Tests;

[TestFixture]
public class AstIntegrationTests
{

    private static string[] GetAstJsonFiles()
    {
        var testDataDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
        return !Directory.Exists(testDataDir) ? [] : Directory.GetFiles(testDataDir, "*.json");
    }

    [TestCaseSource(nameof(GetAstJsonFiles))]
    public void Test_Deserialization_Succeeds(string jsonFilePath)
    {

        Assert.DoesNotThrow(() =>
            {
                var result = SvParser.ParseFromAstJsonFilePath(jsonFilePath);

                Assert.That(result, Is.Not.Null, "Deserialization returned null.");
            }, $"Failed to parse JSON file: {Path.GetFileName(jsonFilePath)}");
    }
}