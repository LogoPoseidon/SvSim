using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Patterns;

public record TaggedPattern : SvPattern
{
    public FieldSymbol? Member;
    public SvPattern? ValuePattern;
};