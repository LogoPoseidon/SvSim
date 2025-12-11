using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Symbols.Types;

public record ClassType : SvType
{
    public GenericClassDefSymbol? GenericClass;
    public SvSymbol[] GenericParameters = [];
    public VariableSymbol? ThisVar;
    public bool? IsAbstract;
    public bool? IsInterface;
    public bool? IsFinal;
    public bool? IsUninstantiated;
};