// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using SvSim.SlangAstParser;

var json = File.ReadAllText("/home/timo/slang/tests/regression/test.json");
var serializerOptions = SlangSerializer.GetOptions();
var readJson = JsonSerializer.Deserialize<Design>(json, serializerOptions);
Console.WriteLine("Hello, World!");