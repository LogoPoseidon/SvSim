using SvSim.SlangAstParser.Ast;
using SvSim.SlangAstParser.Ast.Symbols;

namespace SvSim.SlangAstParser;

public record Design : AstNode
{
    public Root? Root;
    public DefinitionSymbol[] Definitions = [];
};