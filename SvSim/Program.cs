using SvSim.SlangAstParser.Serializer;
using SvSim.Simulation.Engine;
using SvSim.Elaboration;

var json = File.ReadAllText(@"E:\Hardware\SystemVerilogTests\out.json");
var topLevel = SlangSerializer.Parse(json);

var scheduler = new EventScheduler();
var elaborator = new StructuralElaborator(scheduler);

Console.WriteLine("Elaborating Design...");
elaborator.ElaborateDesign(topLevel!.Design);

Console.WriteLine("Starting Simulation Kernel...\n");

scheduler.Run();