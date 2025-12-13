using System.Text.Json;
using SvSim.SlangAstParser;
using SvSim.SlangAstParser.Serializer;

var json = File.ReadAllText(@"E:\Hardware\SystemVerilogTests\testbench.json");
var topLevel = SlangSerializer.Parse(json);
;