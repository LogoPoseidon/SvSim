using SvSim.SlangAstParser.Ast.TimingControls;

namespace SvSim.SlangAstParser.Ast.Statements;

public record TimedStatement : SvStatement
{
    public TimingControl? Timing;
    public SvStatement? Stmt;
};