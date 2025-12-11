using SvSim.SlangAstParser.Ast.Symbols.InstanceSymbolBases;

namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record VirtualInterfaceType : SvType
{
    public InstanceSymbol? Iface;
    public ModportSymbol? ModPort;
    public bool? IsRealIFace;
};