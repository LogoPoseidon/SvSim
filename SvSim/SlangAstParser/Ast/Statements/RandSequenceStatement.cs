using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Statements;

public record RandSequenceStatement : SvStatement
{
    public RandSeqProductionSymbol? FirstProduction;
    public RandSeqProductionSymbol[] Productions = [];
};