using SvSim.SlangAstParser.Ast.Expressions;
using SvSim.SlangAstParser.Enums;

namespace SvSim.SlangAstParser.Ast.Statements;

public record ConditionalStatement : SvStatement
{
    public Condition[] Conditions = [];
    public SvStatement? IfTrue;
    public SvStatement? IfFalse;
    public UniquePriorityCheck? Check;
};