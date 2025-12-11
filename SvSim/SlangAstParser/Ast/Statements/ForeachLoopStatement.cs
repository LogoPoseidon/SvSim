using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ForeachLoopStatement : SvStatement
{
    public SvExpression? ArrayRef;
    public LoopDim[] LoopDims = [];
    public SvStatement? Body;
};

public struct LoopDim
{
    public IteratorSymbol? LoopVar;
}