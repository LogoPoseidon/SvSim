using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Statements;

public record VariableDeclStatement : SvStatement
{
    public VariableSymbol? Symbol;
};