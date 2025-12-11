using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record MultiPortSymbol : SvSymbol
{
    public ArgumentDirection? Direction;
    public bool? IsNullPort;
};