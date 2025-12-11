using SvSim.SlangAstParser.Ast.AssertionExprs;
using SvSim.SlangAstParser.Ast.Symbols;
using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Expressions;

public record AssertionInstanceSvExpression : SvExpression
{
    public SvSymbol? Symbol;
    public AssertionExpr? Body;
    public (AssertionPortSymbol, string)[] Arguments = [];
    public LocalAssertionVarSymbol[] LocalVars = [];
    public bool? IsRecursiveProperty;
};