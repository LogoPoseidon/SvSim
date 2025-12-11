using SvSim.SlangAstParser.Ast.Symbols.InstanceSymbolBases;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Symbols;

public record InstanceBodySymbol : SvSymbol
{
    public InstanceSymbol? ParentInstance;
    public HierarchyOverrideNode? HierarchyOverrideNode;
    public InstanceFlags? Flags;
};