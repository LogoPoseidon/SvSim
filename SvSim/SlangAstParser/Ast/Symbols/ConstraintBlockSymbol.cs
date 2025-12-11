using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record ConstraintBlockSymbol : SvSymbol
{
    public VariableSymbol? ThisVar;
    public ConstraintBlockFlags Flags;
};