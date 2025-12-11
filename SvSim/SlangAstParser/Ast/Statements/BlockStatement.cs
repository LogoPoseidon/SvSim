using SvSim.SlangAstParser.Ast.Symbols;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public record BlockStatement : SvStatement
{
    public SvStatement? Body;
    public StatementBlockSymbol? BlockSymbol;
    public StatementBlockKind? BlockKind;
};