using System.Text.Json;
using SvSim.SlangAstParser;
using SvSim.SlangAstParser.Serializer;

var json = File.ReadAllText(@"/home/timo/git-projects/VerilogTest/test.json");
var topLevel = SlangSerializer.Parse(json);
;