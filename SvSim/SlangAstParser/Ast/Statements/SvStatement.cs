using SvSim.SlangAstParser.Ast.Symbols;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public abstract record SvStatement : AstNode
{
    public StatementKind? Kind;
    public StatementContext? Context;
    public EvalResult? EvalResult;
    public struct StatementContext
    {
        public StatementBlockSymbol[] Blocks;
        public StatementFlags? Flags;
    }
};



public enum EvalResult
{
    Fail,
    Success,
    Return,
    Break,
    Continue,
    Disable 
}