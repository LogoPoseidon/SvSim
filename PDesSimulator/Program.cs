using PDesSimulator.SystemVerilogSimulator;

string[] svFile = [@"SystemVerilogTests/TbAlu.sv",@"SystemVerilogTests/Alu.sv"];
const string vcdOutput = @"SystemVerilogTests/";
const ulong simDuration = 5000;
SvSimulationRunner.RunSimulation(svFile, simDuration, vcdOutput);
