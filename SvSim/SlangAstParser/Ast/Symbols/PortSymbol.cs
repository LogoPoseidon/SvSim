using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record PortSymbol : SvSymbol
{
    public SvSymbol? InternalSymbol;
    public ArgumentDirection? Direction;
    public bool? IsNullPort;
    public bool? IsAnsiPort;
};