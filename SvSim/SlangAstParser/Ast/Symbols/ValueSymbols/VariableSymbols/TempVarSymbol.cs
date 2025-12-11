using SvSim.SlangAstParser.Ast.Symbols.Types;

namespace SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

public abstract record TempVarSymbol : VariableSymbol
{
    
};

public record IteratorSymbol : TempVarSymbol
{
    public SvType? ArrayType;
    public string? IndexMethodName;
}

public record PatternVarSymbol : TempVarSymbol
{
    
}