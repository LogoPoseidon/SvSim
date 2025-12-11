using SvSim.SlangAstParser.Ast.Symbols;
using SvSim.SlangAstParser.Ast.Symbols.InstanceSymbolBases;

namespace SvSim.SlangAstParser.Ast;

public record ResolvedConfig : AstNode
{
    public ConfigBlockSymbol? UseConfig;
    public InstanceSymbol? RootInstance;
    public ConfigRule? ConfigRule;
};