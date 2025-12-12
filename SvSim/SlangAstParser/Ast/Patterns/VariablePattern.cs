using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Patterns;

public record VariablePattern : SvPattern
{
    public PatternVarSymbol? Variable;
};