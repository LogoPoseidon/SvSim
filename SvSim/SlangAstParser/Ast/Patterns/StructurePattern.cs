using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Patterns;

public record StructurePattern : SvPattern
{
    public FieldPattern[] Patterns = [];

    public struct FieldPattern
    {
        public required FieldSymbol Field;
        public required SvPattern Pattern;
    }
};