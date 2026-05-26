using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.Constraint;
using SvSim.SlangAstParser.AstTree.Expression;
using SvSim.SlangAstParser.AstTree.RandSeqProductionProd;
using SvSim.SlangAstParser.AstTree.Scope;
using SvSim.SlangAstParser.AstTree.Statement;
using SvSim.SlangAstParser.AstTree.SvEnums;
using SvSim.SlangAstParser.AstTree.Symbol.InstanceSymbolBase;
using SvSim.SlangAstParser.AstTree.Symbol.Type;
using SvSim.SlangAstParser.AstTree.Symbol.Type.IntegralType;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol;
using SvSim.SlangAstParser.AstTree.Symbol.ValueSymbol.VariableSymbol.TempVarSymbol;
using SvSim.SlangAstParser.AstTree.TimingControl;

namespace SvSim.SlangAstParser.AstTree.Symbol;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvRoot), nameof(SvSymbolKind.Root))]
[JsonDerivedType(typeof(SvCompilationUnit), nameof(SvSymbolKind.CompilationUnit))]
[JsonDerivedType(typeof(SvPackage), nameof(SvSymbolKind.Package))]
[JsonDerivedType(typeof(SvAttribute), nameof(SvSymbolKind.Attribute))]
[JsonDerivedType(typeof(SvParameter), nameof(SvSymbolKind.Parameter))]
[JsonDerivedType(typeof(SvAnonymousProgram), nameof(SvSymbolKind.AnonymousProgram))]
[JsonDerivedType(typeof(SvPrimitive), nameof(SvSymbolKind.Primitive))]
[JsonDerivedType(typeof(SvPrimitivePort), nameof(SvSymbolKind.PrimitivePort))]
[JsonDerivedType(typeof(SvConfigBlock), nameof(SvSymbolKind.ConfigBlock))]
[JsonDerivedType(typeof(SvEnumValue), nameof(SvSymbolKind.EnumValue))]
[JsonDerivedType(typeof(SvChecker), nameof(SvSymbolKind.Checker))]
[JsonDerivedType(typeof(SvAssertionPort), nameof(SvSymbolKind.AssertionPort))]
[JsonDerivedType(typeof(SvClassProperty), nameof(SvSymbolKind.ClassProperty))]
[JsonDerivedType(typeof(SvMethodPrototype), nameof(SvSymbolKind.MethodPrototype))]
[JsonDerivedType(typeof(SvFormalArgument), nameof(SvSymbolKind.FormalArgument))]
[JsonDerivedType(typeof(SvSubroutine), nameof(SvSymbolKind.Subroutine))]
[JsonDerivedType(typeof(SvVariable), nameof(SvSymbolKind.Variable))]
[JsonDerivedType(typeof(SvGenericClassDef), nameof(SvSymbolKind.GenericClassDef))]
[JsonDerivedType(typeof(SvConstraintBlock), nameof(SvSymbolKind.ConstraintBlock))]
[JsonDerivedType(typeof(SvIterator), nameof(SvSymbolKind.Iterator))]
[JsonDerivedType(typeof(SvCovergroup), nameof(SvSymbolKind.CovergroupType))]
[JsonDerivedType(typeof(SvCovergroupBody), nameof(SvSymbolKind.CovergroupBody))]
[JsonDerivedType(typeof(SvCoverpoint), nameof(SvSymbolKind.Coverpoint))]
[JsonDerivedType(typeof(SvCoverageBin), nameof(SvSymbolKind.CoverageBin))]
[JsonDerivedType(typeof(SvCoverCross), nameof(SvSymbolKind.CoverCross))]
[JsonDerivedType(typeof(SvCoverCrossBody), nameof(SvSymbolKind.CoverCrossBody))]
[JsonDerivedType(typeof(SvInstance), nameof(SvSymbolKind.Instance))]
[JsonDerivedType(typeof(SvInstanceBody), nameof(SvSymbolKind.InstanceBody))]
[JsonDerivedType(typeof(SvDefinition), nameof(SvSymbolKind.Definition))]
[JsonDerivedType(typeof(SvPort), nameof(SvSymbolKind.Port))]
[JsonDerivedType(typeof(SvNet), nameof(SvSymbolKind.Net))]
[JsonDerivedType(typeof(SvNetType), nameof(SvSymbolKind.NetType))]
[JsonDerivedType(typeof(SvSpecifyBlock), nameof(SvSymbolKind.SpecifyBlock))]
[JsonDerivedType(typeof(SvSpecparam), nameof(SvSymbolKind.Specparam))]
[JsonDerivedType(typeof(SvTimingPath), nameof(SvSymbolKind.TimingPath))]
[JsonDerivedType(typeof(SvModport), nameof(SvSymbolKind.Modport))]
[JsonDerivedType(typeof(SvModportPort), nameof(SvSymbolKind.ModportPort))]
[JsonDerivedType(typeof(SvInterfacePort), nameof(SvSymbolKind.InterfacePort))]
[JsonDerivedType(typeof(SvMultiPort), nameof(SvSymbolKind.MultiPort))]
[JsonDerivedType(typeof(SvContinuousAssign), nameof(SvSymbolKind.ContinuousAssign))]
[JsonDerivedType(typeof(SvNetAlias), nameof(SvSymbolKind.NetAlias))]
[JsonDerivedType(typeof(SvGenvar), nameof(SvSymbolKind.Genvar))]
[JsonDerivedType(typeof(SvGenerateBlockArray), nameof(SvSymbolKind.GenerateBlockArray))]
[JsonDerivedType(typeof(SvGenerateBlock), nameof(SvSymbolKind.GenerateBlock))]
[JsonDerivedType(typeof(SvEmptyMember), nameof(SvSymbolKind.EmptyMember))]
[JsonDerivedType(typeof(SvProperty), nameof(SvSymbolKind.Property))]
[JsonDerivedType(typeof(SvPrimitiveInstance), nameof(SvSymbolKind.PrimitiveInstance))]
[JsonDerivedType(typeof(SvDefParam), nameof(SvSymbolKind.DefParam))]
[JsonDerivedType(typeof(SvClockingBlock), nameof(SvSymbolKind.ClockingBlock))]
[JsonDerivedType(typeof(SvClockVar), nameof(SvSymbolKind.ClockVar))]
[JsonDerivedType(typeof(SvCheckerInstance), nameof(SvSymbolKind.CheckerInstance))]
[JsonDerivedType(typeof(SvCheckerInstanceBody), nameof(SvSymbolKind.CheckerInstanceBody))]
[JsonDerivedType(typeof(SvSequence), nameof(SvSymbolKind.Sequence))]
[JsonDerivedType(typeof(SvRandSeqProduction), nameof(SvSymbolKind.RandSeqProduction))]
[JsonDerivedType(typeof(SvTransparentMember), nameof(SvSymbolKind.TransparentMember))]
[JsonDerivedType(typeof(SvModportClocking), nameof(SvSymbolKind.ModportClocking))]
[JsonDerivedType(typeof(SvInstanceArray), nameof(SvSymbolKind.InstanceArray))]
[JsonDerivedType(typeof(SvExplicitImport), nameof(SvSymbolKind.ExplicitImport))]
[JsonDerivedType(typeof(SvField), nameof(SvSymbolKind.Field))]
[JsonDerivedType(typeof(SvPatternVar), nameof(SvSymbolKind.PatternVar))]
[JsonDerivedType(typeof(SvLocalAssertionVar), nameof(SvSymbolKind.LocalAssertionVar))]
[JsonDerivedType(typeof(SvLetDecl), nameof(SvSymbolKind.LetDecl))]
[JsonDerivedType(typeof(SvPulseStyle), nameof(SvSymbolKind.PulseStyle))]
[JsonDerivedType(typeof(SvSystemTimingCheck), nameof(SvSymbolKind.SystemTimingCheck))]
[JsonDerivedType(typeof(SvPredefinedInteger), nameof(SvSymbolKind.PredefinedIntegerType))]
[JsonDerivedType(typeof(SvScalar), nameof(SvSymbolKind.ScalarType))]
[JsonDerivedType(typeof(SvFloatingType), nameof(SvSymbolKind.FloatingType))]
[JsonDerivedType(typeof(SvEnum), nameof(SvSymbolKind.EnumType))]
[JsonDerivedType(typeof(SvPackedArray), nameof(SvSymbolKind.PackedArrayType))]
[JsonDerivedType(typeof(SvFixedSizeUnpackedArrayType), nameof(SvSymbolKind.FixedSizeUnpackedArrayType))]
[JsonDerivedType(typeof(SvDynamicArrayType), nameof(SvSymbolKind.DynamicArrayType))]
[JsonDerivedType(typeof(SvDpiOpenArrayType), nameof(SvSymbolKind.DPIOpenArrayType))]
[JsonDerivedType(typeof(SvAssociativeArrayType), nameof(SvSymbolKind.AssociativeArrayType))]
[JsonDerivedType(typeof(SvQueueType), nameof(SvSymbolKind.QueueType))]
[JsonDerivedType(typeof(SvPackedStruct), nameof(SvSymbolKind.PackedStructType))]
[JsonDerivedType(typeof(SvUnpackedStructType), nameof(SvSymbolKind.UnpackedStructType))]
[JsonDerivedType(typeof(SvPackedUnion), nameof(SvSymbolKind.PackedUnionType))]
[JsonDerivedType(typeof(SvUnpackedUnionType), nameof(SvSymbolKind.UnpackedUnionType))]
[JsonDerivedType(typeof(SvClassType), nameof(SvSymbolKind.ClassType))]  
[JsonDerivedType(typeof(SvVoidType), nameof(SvSymbolKind.VoidType))]
[JsonDerivedType(typeof(SvNullType), nameof(SvSymbolKind.NullType))]
[JsonDerivedType(typeof(SvCHandleType), nameof(SvSymbolKind.CHandleType))]
[JsonDerivedType(typeof(SvStringType), nameof(SvSymbolKind.StringType))]
[JsonDerivedType(typeof(SvEventType), nameof(SvSymbolKind.EventType))]
[JsonDerivedType(typeof(SvUnboundedType), nameof(SvSymbolKind.UnboundedType))]
[JsonDerivedType(typeof(SvTypeRefType), nameof(SvSymbolKind.TypeRefType))]
[JsonDerivedType(typeof(SvUntypedType), nameof(SvSymbolKind.UntypedType))]
[JsonDerivedType(typeof(SvSequenceType), nameof(SvSymbolKind.SequenceType))]
[JsonDerivedType(typeof(SvPropertyType), nameof(SvSymbolKind.PropertyType))]
[JsonDerivedType(typeof(SvVirtualInterfaceType), nameof(SvSymbolKind.VirtualInterfaceType))]
[JsonDerivedType(typeof(SvTypeAlias), nameof(SvSymbolKind.TypeAlias))]
[JsonDerivedType(typeof(SvErrorType), nameof(SvSymbolKind.ErrorType))]
[JsonDerivedType(typeof(SvForwardingTypedef), nameof(SvSymbolKind.ForwardingTypedef))]
[JsonDerivedType(typeof(SvProceduralBlock), nameof(SvSymbolKind.ProceduralBlock))]
[JsonDerivedType(typeof(SvStatementBlock), nameof(SvSymbolKind.StatementBlock))]
[JsonDerivedType(typeof(SvElabSystemTask), nameof(SvSymbolKind.ElabSystemTask))]
[JsonDerivedType(typeof(SvWildcardImport), nameof(SvSymbolKind.WildcardImport))]
[JsonDerivedType(typeof(SvUninstantiatedDef), nameof(SvSymbolKind.UninstantiatedDef))]
[JsonDerivedType(typeof(SvUnknown), nameof(SvSymbolKind.Unknown))]
[JsonDerivedType(typeof(SvTypeParameter), nameof(SvSymbolKind.TypeParameter))]


public interface ISvSymbol : ISvAstNode
{
    public string Name { get; init; }
    public long Addr { get; init; }
}

public record SvAnonymousProgram : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvAssertionPort : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}
public record SvAttribute : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Value { get; init; }
    public string? Kind { get; init; }
}
public record SvCheckerInstanceBody : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }

    public string? Kind { get; init; }
}
public record SvChecker : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvClockingBlock : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public ISvTimingControl? Event { get; init; }
    public SvClockingSkew? DefaultInputSkew { get; init; }
    public SvClockingSkew? DefaultOutputSkew { get; init; }
    public string? Kind { get; init; }
}
public record SvClockingSkew(ISvTimingControl? Delay, SvEdgeKind? Edge);


public record SvCompilationUnit : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvConfigBlock : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvConstraintBlock : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public ISvConstraint? Constraints { get; init; }
    public string? Kind { get; init; }
    public SvConstraintBlockFlags Flags { get; init; }
}
public record SvContinuousAssign : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvExpression? Assignment { get; init; }
    public ISvTimingControl? Delay { get; init; }
    public string? DriveStrength0 { get; init; }
    public string? DriveStrength1 { get; init; }
    public string? Kind { get; init; }
}

public record SvCoverCrossBody : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvCoverCross : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public CoverCrossTarget[]? Targets { get; init; }
    public CoverOption[]? Options { get; init; }
    public string? Kind { get; init; }
}
public record CoverOption(ISvExpression Expr);

public record CoverCrossTarget(string Coverpoint);

public record SvCoverageBin : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? BinsKind { get; init; }
    public bool IsArray { get; init; }
    public bool IsWildcard { get; init; }
    public bool IsDefault { get; init; }
    public bool IsDefaultSequence { get; init; }
    public ISvExpression[]? Values { get; init; }
    public CoverageBinTransItem[][]? Trans { get; init; }
    public ISvExpression? CrossSelect { get; init; }
    public string? Kind { get; init; }
}
public record CoverageBinTransItem(ISvExpression[] Items, ISvExpression? RepeatFrom, ISvExpression? RepeatTo, string? RepeatKind);

public record SvCovergroupBody : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public CoverOption[]? Options { get; init; }
    public string? Kind { get; init; }
}
public record SvCoverpoint : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public CoverOption[]? Options { get; init; }
    public ISvExpression? Iff { get; init; }
    public string? Kind { get; init; }
}

public record SvDefParam : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Target { get; init; }
    public ISvExpression? Value { get; init; }
    public string? Kind { get; init; }
}

public record SvDefinition : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? DefaultNetType { get; init; }
    public SvDefinitionKind? DefinitionKind { get; init; }
    public SvVariableLifetime? DefaultLifeTime { get; init; }
    public SvUnconnectedDrive? UnconnectedDrive { get; init; }
    public bool? CellDefine { get; init; }
    public string? TimeScale { get; init; }
    public SvAttribute[]? Attributes { get; init; }
    public string? Kind { get; init; }
}
public record SvTimeScale
{
    public required string Base { get; init; }
    public required string Precision { get; init; }
}

public record SvElabSystemTask : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvElabSystemTaskKind? TaskKind { get; init; }
    public string? Message { get; init; }
    public string? Kind { get; init; }
}

public record SvEmptyMember : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}

public record SvExplicitImport : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Package { get; init; }
    public string? Kind { get; init; }
}

public record SvForwardingTypedef : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvForwardTypeRestriction? Restriction { get; init; }
    public SvVisibility? Visibility { get; init; }
    public ISvSymbol? Next { get; init; }
    public string? Kind { get; init; }
    public string? Category { get; init; }
}

public record SvGenerateBlockArray : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public int ConstructIndex { get; init; }
    public string? Kind { get; init; }
}
public record SvGenerateBlock : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public int ConstructIndex { get; init; }
    public bool IsUninstantiated { get; init; }
    public string? Kind { get; init; }
}
public record SvGenericClassDef : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public ISvSymbol[]? Specializations { get; init; }
    public SvParameter[]? Parameters { get; init; }
    public bool IsInterface { get; init; }
    public string? Kind { get; init; }
}
public record SvGenvar : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}
public record SvInstanceArray : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; } 
    public required string Range { get; init; }
    public string? Kind { get; init; }
}
public record SvInstanceBody : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public required string Definition { get; init; }
    public string? Kind { get; init; }
}
public record SvInterfacePort : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? InterfaceDef { get; init; }
    public string? Modport { get; init; }
    public bool IsGeneric { get; init; }
    public string? Kind { get; init; }
}
public record SvInvalid : ISvSymbol
{
    public string? Kind { get; init; }
    public required string Name { get; init; }
    public long Addr { get; init; }
    
}

public record SvLetDecl : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvMethodPrototype : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? ReturnType { get; init; }
    public SvSubroutineKind? SubroutineKind { get; init; }
    public SvVisibility? Visibility { get; init; }
    public ISvSymbol[]? Arguments { get; init; }
    public SvMethodFlags? Flags { get; init; }
    public ISvSymbol? Subroutine { get; init; }
    public string? Kind { get; init; }
}
public record SvModportClocking : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Target { get; init; }
    public string? Kind { get; init; }
}
public record SvModport : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvMultiPort : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public SvArgumentDirection? Direction { get; init; }
    public bool IsNullPort { get; init; }
    public MultiPortConnection[]? Ports { get; init; }
    public string? Kind { get; init; }
}
public record MultiPortConnection(string Type, string Direction, string InternalSymbol);

public record SvNetAlias : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvExpression[]? NetReferences { get; init; }
    public string? Kind { get; init; }
}
public record SvNetType : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public SvNetKind? NetKind { get; init; }
    public string? ResolutionFunction { get; init; }
    public string? Kind { get; init; }
}
public record SvPackage : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public SvAttribute[]? Attributes { get; init; }
    public string? Kind { get; init; }
}
public record SvPort : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public SvArgumentDirection? Direction { get; init; }
    public string? InternalSymbol { get; init; }
    public bool IsNullPort { get; init; }
    public ISvExpression? Initializer { get; init; }
    public SvAttribute[]? Attributes { get; init; }
    public string? Kind { get; init; }
}
public record SvPrimitive : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public bool IsSequential { get; init; }
    public PrimitiveTableEntry[]? Table { get; init; }
    public string? Kind { get; init; }
}
public record PrimitiveTableEntry(string Inputs, string State, string Output);

public record SvProceduralBlock : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public SvProceduralBlockKind? ProcedureKind { get; init; }
    public ISvStatement? Body { get; init; }
    public string? Kind { get; init; }
}
public record SvProperty : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvPulseStyle : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}
public record SvRandSeqProduction : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? ReturnType { get; init; }
    public ISvSymbol[]? Arguments { get; init; }
    public RandSeqRule[]? Rules { get; init; }
    public string? Kind { get; init; }
}
public record RandSeqRule
{
    public required IRandSeqProductionProd[] Prods { get; init; }
    public ISvExpression? WeightExpr { get; init; }
    public bool? IsRandJoin { get; init; }
    public ISvExpression? RandJoinExpr { get; init; }
}

public record SvRoot : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvSequence : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}

public record SvSpecifyBlock : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvStatementBlock : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? Kind { get; init; }
}
public record SvSubroutine : ISvSymbol, ISvScope
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public ISvSymbol[]? Members { get; init; }
    public string? ReturnType { get; init; }
    public SvVariableLifetime? DefaultLifetime { get; init; }
    public SvSubroutineKind? SubroutineKind { get; init; }
    public ISvStatement? Body { get; init; }
    public SvVisibility? Visibility { get; init; }
    public ISvSymbol[]? Arguments { get; init; }
    public string? Flags { get; init; }
    public string? Kind { get; init; }
}
public record SvSystemTimingCheck : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}
public record SvTimingPath : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? ConnectionKind { get; init; }
    public string? Polarity { get; init; }
    public string? EdgePolarity { get; init; }
    public string? EdgeIdentifier { get; init; }
    public bool IsStateDependent { get; init; }
    public ISvExpression? EdgeSourceExpr { get; init; }
    public ISvExpression? ConditionExpr { get; init; }
    public ISvExpression[]? Inputs { get; init; }
    public ISvExpression[]? Outputs { get; init; }
    public ISvExpression[]? Delays { get; init; }
    public string? Kind { get; init; }
}
public record SvTransparentMember : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}
public record SvWildcardImport : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
    public bool IsFromExport { get; init; }
    public string? Package { get; init; }
}
public record SvUnknown : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Kind { get; init; }
}
public record SvUninstantiatedDef : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public required string[] PortConnections { get; init; }
    public required string[] PortNames { get; init; }
    public bool IsChecker { get; init; }
    public required string DefinitionName { get; init; }
    public required string ParamExpressions { get; init; }
    public string? Kind { get; init; }
}
public record SvTypeParameter : ISvSymbol
{
    public required string Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public bool IsLocal { get; init; }
    public bool IsPort { get; init; }
    public bool IsBody { get; init; }
    public string? Kind { get; init; }
}