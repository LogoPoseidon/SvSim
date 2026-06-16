using SvAstParser;
using SvDesSim.Elaboration;
using SvDesSim.Simulation.Engine;


var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
var svFilePath = Path.Combine(baseDirectory, "TestProgram/testbenchSimulator.sv");

Console.WriteLine($"Looking for SystemVerilog file at: {svFilePath}");

var topLevel = SvParser.ParseFromSystemVerilogFilePath(svFilePath);
Console.WriteLine("AST generation and parsing completed.");

var scheduler = new EventScheduler();
var elaborator = new StructuralElaborator(scheduler);

Console.WriteLine("Elaborating Design...");
elaborator.ElaborateDesign(topLevel);

Console.WriteLine("Starting Simulation Kernel...\n");
scheduler.Run();