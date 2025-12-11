using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ProceduralCheckerStatement : SvStatement
{
    public SvSymbol[] Instances = [];
};