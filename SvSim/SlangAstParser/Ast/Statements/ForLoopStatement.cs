using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Ast.Symbols.ValueSymbols.VariableSymbols;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ForLoopStatement : SvStatement
{
    public SvExpression[] Initializers = [];
    public VariableSymbol[] LoopVars = [];
    public SvExpression? StopExpr;
    public SvExpression[] Steps = [];
    public SvStatement? Body;
};