using System.Text.Json.Serialization;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.SlangAstParser.AstTree;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SvRoot), "Root")]
[JsonDerivedType(typeof(SvCompilationUnit), "CompilationUnit")]
[JsonDerivedType(typeof(SvPackage), "Package")]
[JsonDerivedType(typeof(SvAttribute), "Attribute")]
[JsonDerivedType(typeof(SvParameter), "Parameter")]
[JsonDerivedType(typeof(SvIntegerLiteral), "IntegerLiteral")]
[JsonDerivedType(typeof(SvTypeParameter), "TypeParameter")]
[JsonDerivedType(typeof(SvAnonymousProgram), "AnonymousProgram")]
[JsonDerivedType(typeof(SvPrimitive), "Primitive")]
[JsonDerivedType(typeof(SvPrimitivePort), "PrimitivePort")]
[JsonDerivedType(typeof(SvConfigBlock), "ConfigBlock")]
[JsonDerivedType(typeof(SvConversion), "Conversion")]
[JsonDerivedType(typeof(SvEnumType), "EnumType")]
[JsonDerivedType(typeof(SvEnumValue), "EnumValue")]
[JsonDerivedType(typeof(SvTypeAlias), "TypeAlias")]
[JsonDerivedType(typeof(SvChecker), "Checker")]
[JsonDerivedType(typeof(SvAssertionPort), "AssertionPort")]
[JsonDerivedType(typeof(SvClassType), "ClassType")]
[JsonDerivedType(typeof(SvClassProperty), "ClassProperty")]
[JsonDerivedType(typeof(SvMethodPrototype), "MethodPrototype")]
[JsonDerivedType(typeof(SvFormalArgument), "FormalArgument")]
[JsonDerivedType(typeof(SvSubroutine), "Subroutine")]
[JsonDerivedType(typeof(SvVariable), "Variable")]
[JsonDerivedType(typeof(SvExpressionStatement), "ExpressionStatement")]
[JsonDerivedType(typeof(SvAssignment), "Assignment")]
[JsonDerivedType(typeof(SvNamedValue), "NamedValue")]
[JsonDerivedType(typeof(SvBinaryOp), "BinaryOp")]
[JsonDerivedType(typeof(SvList), "List")]
[JsonDerivedType(typeof(SvGenericClassDef), "GenericClassDef")]
[JsonDerivedType(typeof(SvReturn), "Return")]
[JsonDerivedType(typeof(SvCall), "Call")]
[JsonDerivedType(typeof(SvConstraintBlock), "ConstraintBlock")]
[JsonDerivedType(typeof(SvExpression), "Expression")]
[JsonDerivedType(typeof(SvDist), "Dist")]
[JsonDerivedType(typeof(SvPerRange), "PerRange")]
[JsonDerivedType(typeof(SvPerValue), "PerValue")]
[JsonDerivedType(typeof(SvElementSelect), "ElementSelect")]
[JsonDerivedType(typeof(SvInside), "Inside")]
[JsonDerivedType(typeof(SvRangeSelect), "RangeSelect")]
[JsonDerivedType(typeof(SvSolveBefore), "SolveBefore")]
[JsonDerivedType(typeof(SvImplication), "Implication")]
[JsonDerivedType(typeof(SvConditional), "Conditional")]
[JsonDerivedType(typeof(SvForeach), "Foreach")]
[JsonDerivedType(typeof(SvIterator), "Iterator")]
[JsonDerivedType(typeof(SvDisableSoft), "DisableSoft")]
[JsonDerivedType(typeof(SvCovergroupType), "CovergroupType")]
[JsonDerivedType(typeof(SvCovergroupBody), "CovergroupBody")]
[JsonDerivedType(typeof(SvCoverpoint), "Coverpoint")]
[JsonDerivedType(typeof(SvCoverageBin), "CoverageBin")]
[JsonDerivedType(typeof(SvValueRange), "ValueRange")]
[JsonDerivedType(typeof(SvUnboundedLiteral), "UnboundedLiteral")]
[JsonDerivedType(typeof(SvCoverCross), "CoverCross")]
[JsonDerivedType(typeof(SvCoverCrossBody), "CoverCrossBody")]
[JsonDerivedType(typeof(SvSetExpr), "SetExpr")]
[JsonDerivedType(typeof(SvSimpleAssignmentPattern), "SimpleAssignmentPattern")]
[JsonDerivedType(typeof(SvWithFilter), "WithFilter")]
[JsonDerivedType(typeof(SvBinary), "Binary")]
[JsonDerivedType(typeof(SvUnary), "Unary")]
[JsonDerivedType(typeof(SvCondition), "Condition")]
[JsonDerivedType(typeof(SvSignalEvent), "SignalEvent")]
[JsonDerivedType(typeof(SvInstance), "Instance")]
[JsonDerivedType(typeof(SvInstanceBody), "InstanceBody")]
[JsonDerivedType(typeof(SvPort), "Port")]
[JsonDerivedType(typeof(SvNet), "Net")]
[JsonDerivedType(typeof(SvNetType), "NetType")]
[JsonDerivedType(typeof(SvSpecifyBlock), "SpecifyBlock")]
[JsonDerivedType(typeof(SvSpecparam), "Specparam")]
[JsonDerivedType(typeof(SvTimingPath), "TimingPath")]
[JsonDerivedType(typeof(SvRealLiteral), "RealLiteral")]
[JsonDerivedType(typeof(SvStatementBlock), "StatementBlock")]
[JsonDerivedType(typeof(SvProceduralBlock), "ProceduralBlock")]
[JsonDerivedType(typeof(SvBlock), "Block")]
[JsonDerivedType(typeof(SvVariableDeclaration), "VariableDeclaration")]
[JsonDerivedType(typeof(SvStreaming), "Streaming")]
[JsonDerivedType(typeof(SvMemberAccess), "MemberAccess")]
[JsonDerivedType(typeof(SvNewClass), "NewClass")]
[JsonDerivedType(typeof(SvModport), "Modport")]
[JsonDerivedType(typeof(SvModportPort), "ModportPort")]
[JsonDerivedType(typeof(SvInterfacePort), "InterfacePort")]
[JsonDerivedType(typeof(SvWildcardImport), "WildcardImport")]
[JsonDerivedType(typeof(SvMultiPort), "MultiPort")]
[JsonDerivedType(typeof(SvUnbasedUnsizedIntegerLiteral), "UnbasedUnsizedIntegerLiteral")]
[JsonDerivedType(typeof(SvElabSystemTask), "ElabSystemTask")]
[JsonDerivedType(typeof(SvContinuousAssign), "ContinuousAssign")]
[JsonDerivedType(typeof(SvDelay3), "Delay3")]
[JsonDerivedType(typeof(SvMinTypMax), "MinTypMax")]
[JsonDerivedType(typeof(SvNetAlias), "NetAlias")]
[JsonDerivedType(typeof(SvConcatenation), "Concatenation")]
[JsonDerivedType(typeof(SvRepeatLoop), "RepeatLoop")]
[JsonDerivedType(typeof(SvTimed), "Timed")]
[JsonDerivedType(typeof(SvDelay), "Delay")]
[JsonDerivedType(typeof(SvWait), "Wait")]
[JsonDerivedType(typeof(SvUnaryOp), "UnaryOp")]
[JsonDerivedType(typeof(SvWaitFork), "WaitFork")]
[JsonDerivedType(typeof(SvWaitOrder), "WaitOrder")]
[JsonDerivedType(typeof(SvHierarchicalValue), "HierarchicalValue")]
[JsonDerivedType(typeof(SvDisable), "Disable")]
[JsonDerivedType(typeof(SvArbitrarySymbol), "ArbitrarySymbol")]
[JsonDerivedType(typeof(SvProceduralAssign), "ProceduralAssign")]
[JsonDerivedType(typeof(SvProceduralDeassign), "ProceduralDeassign")]
[JsonDerivedType(typeof(SvCase), "Case")]
[JsonDerivedType(typeof(SvEmpty), "Empty")]
[JsonDerivedType(typeof(SvForeverLoop), "ForeverLoop")]
[JsonDerivedType(typeof(SvBreak), "Break")]
[JsonDerivedType(typeof(SvContinue), "Continue")]
[JsonDerivedType(typeof(SvWhileLoop), "WhileLoop")]
[JsonDerivedType(typeof(SvForLoop), "ForLoop")]
[JsonDerivedType(typeof(SvLValueReference), "LValueReference")]
[JsonDerivedType(typeof(SvForeachLoop), "ForeachLoop")]
[JsonDerivedType(typeof(SvStringLiteral), "StringLiteral")]
[JsonDerivedType(typeof(SvPatternCase), "PatternCase")]
[JsonDerivedType(typeof(SvTagged), "Tagged")]
[JsonDerivedType(typeof(SvStructure), "Structure")]
[JsonDerivedType(typeof(SvWildcard), "Wildcard")]
[JsonDerivedType(typeof(SvConstant), "Constant")]
[JsonDerivedType(typeof(SvConditionalOp), "ConditionalOp")]
[JsonDerivedType(typeof(SvGenvar), "Genvar")]
[JsonDerivedType(typeof(SvGenerateBlockArray), "GenerateBlockArray")]
[JsonDerivedType(typeof(SvGenerateBlock), "GenerateBlock")]
[JsonDerivedType(typeof(SvEmptyMember), "EmptyMember")]
[JsonDerivedType(typeof(SvImmediateAssertion), "ImmediateAssertion")]
[JsonDerivedType(typeof(SvProperty), "Property")]
[JsonDerivedType(typeof(SvConcurrentAssertion), "ConcurrentAssertion")]
[JsonDerivedType(typeof(SvSimple), "Simple")]
[JsonDerivedType(typeof(SvAssertionInstance), "AssertionInstance")]
[JsonDerivedType(typeof(SvClocking), "Clocking")]
[JsonDerivedType(typeof(SvSequenceConcat), "SequenceConcat")]
[JsonDerivedType(typeof(SvPrimitiveInstance), "PrimitiveInstance")]
[JsonDerivedType(typeof(SvOneStepDelay), "OneStepDelay")]
[JsonDerivedType(typeof(SvDefParam), "DefParam")]
[JsonDerivedType(typeof(SvClockingBlock), "ClockingBlock")]
[JsonDerivedType(typeof(SvClockVar), "ClockVar")]
[JsonDerivedType(typeof(SvEventList), "EventList")]
[JsonDerivedType(typeof(SvCheckerInstance), "CheckerInstance")]
[JsonDerivedType(typeof(SvCheckerInstanceBody), "CheckerInstanceBody")]
[JsonDerivedType(typeof(SvProceduralChecker), "ProceduralChecker")]
[JsonDerivedType(typeof(SvSequence), "Sequence")]
[JsonDerivedType(typeof(SvRandSeqProduction), "RandSeqProduction")]
[JsonDerivedType(typeof(SvItem), "Item")]
[JsonDerivedType(typeof(SvRepeat), "Repeat")]
[JsonDerivedType(typeof(SvIfElse), "IfElse")]
[JsonDerivedType(typeof(SvEmptyArgument), "EmptyArgument")]
[JsonDerivedType(typeof(SvCodeBlock), "CodeBlock")]
[JsonDerivedType(typeof(SvDefinition), "Definition")]
[JsonDerivedType(typeof(SvNewArray), "NewArray")]
[JsonDerivedType(typeof(SvNewCovergroup), "NewCovergroup")]
[JsonDerivedType(typeof(SvCopyClass), "CopyClass")]
[JsonDerivedType(typeof(SvTimeLiteral), "TimeLiteral")]
[JsonDerivedType(typeof(SvNullLiteral), "NullLiteral")]
[JsonDerivedType(typeof(SvReplication), "Replication")]
[JsonDerivedType(typeof(SvDataType), "DataType")]
[JsonDerivedType(typeof(SvTypeReference), "TypeReference")]
[JsonDerivedType(typeof(SvStructuredAssignmentPattern), "StructuredAssignmentPattern")]
[JsonDerivedType(typeof(SvReplicatedAssignmentPattern), "ReplicatedAssignmentPattern")]
[JsonDerivedType(typeof(SvClockingEvent), "ClockingEvent")]
[JsonDerivedType(typeof(SvTaggedUnion), "TaggedUnion")]
[JsonDerivedType(typeof(SvDoWhileLoop), "DoWhileLoop")]
[JsonDerivedType(typeof(SvDisableFork), "DisableFork")]
[JsonDerivedType(typeof(SvEventTrigger), "EventTrigger")]
[JsonDerivedType(typeof(SvRandCase), "RandCase")]
[JsonDerivedType(typeof(SvRandSequence), "RandSequence")]
[JsonDerivedType(typeof(SvDeferredMember), "DeferredMember")]
[JsonDerivedType(typeof(SvTransparentMember), "TransparentMember")]
[JsonDerivedType(typeof(SvPredefinedIntegerType), "PredefinedIntegerType")]
[JsonDerivedType(typeof(SvScalarType), "ScalarType")]
[JsonDerivedType(typeof(SvFloatingType), "FloatingType")]
[JsonDerivedType(typeof(SvPackedArrayType), "PackedArrayType")]
[JsonDerivedType(typeof(SvFixedSizeUnpackedArrayType), "FixedSizeUnpackedArrayType")]
[JsonDerivedType(typeof(SvDynamicArrayType), "DynamicArrayType")]
[JsonDerivedType(typeof(SvDpiOpenArrayType), "DPIOpenArrayType")]
[JsonDerivedType(typeof(SvAssociativeArrayType), "AssociativeArrayType")]
[JsonDerivedType(typeof(SvQueueType), "QueueType")]
[JsonDerivedType(typeof(SvPackedStructType), "PackedStructType")]
[JsonDerivedType(typeof(SvUnpackedStructType), "UnpackedStructType")]
[JsonDerivedType(typeof(SvPackedUnionType), "PackedUnionType")]
[JsonDerivedType(typeof(SvUnpackedUnionType), "UnpackedUnionType")]
[JsonDerivedType(typeof(SvVoidType), "VoidType")]
[JsonDerivedType(typeof(SvNullType), "NullType")]
[JsonDerivedType(typeof(SvCHandleType), "CHandleType")]
[JsonDerivedType(typeof(SvStringType), "StringType")]
[JsonDerivedType(typeof(SvEventType), "EventType")]
[JsonDerivedType(typeof(SvUnboundedType), "UnboundedType")]
[JsonDerivedType(typeof(SvTypeRefType), "TypeRefType")]
[JsonDerivedType(typeof(SvUntypedType), "UntypedType")]
[JsonDerivedType(typeof(SvSequenceType), "SequenceType")]
[JsonDerivedType(typeof(SvPropertyType), "PropertyType")]
[JsonDerivedType(typeof(SvVirtualInterfaceType), "VirtualInterfaceType")]
[JsonDerivedType(typeof(SvErrorType), "ErrorType")]
[JsonDerivedType(typeof(SvForwardingTypedef), "ForwardingTypedef")]
[JsonDerivedType(typeof(SvModportClocking), "ModportClocking")]
[JsonDerivedType(typeof(SvInstanceArray), "InstanceArray")]
[JsonDerivedType(typeof(SvExplicitImport), "ExplicitImport")]
[JsonDerivedType(typeof(SvField), "Field")]
[JsonDerivedType(typeof(SvUninstantiatedDef), "UninstantiatedDef")]
[JsonDerivedType(typeof(SvPatternVar), "PatternVar")]
[JsonDerivedType(typeof(SvLocalAssertionVar), "LocalAssertionVar")]
[JsonDerivedType(typeof(SvLetDecl), "LetDecl")]
[JsonDerivedType(typeof(SvPulseStyle), "PulseStyle")]
[JsonDerivedType(typeof(SvSystemTimingCheck), "SystemTimingCheck")]
[JsonDerivedType(typeof(SvUniqueness), "Uniqueness")]
[JsonDerivedType(typeof(SvSequenceWithMatch), "SequenceWithMatch")]
[JsonDerivedType(typeof(SvFirstMatch), "FirstMatch")]
[JsonDerivedType(typeof(SvStrongWeak), "StrongWeak")]
[JsonDerivedType(typeof(SvAbort), "Abort")]
[JsonDerivedType(typeof(SvDisableIff), "DisableIff")]
[JsonDerivedType(typeof(SvCrossId), "CrossId")]
[JsonDerivedType(typeof(SvTempVar), "TempVar")]
public interface IKind
{
}

public record PrimitiveTableEntry(string Inputs, string State, string Output);

public record InstanceConnection(
    IKind? Port,
    IKind? Expr,
    string? IfaceInstance,
    string? Modport,
    string? Formal,
    IKind? Actual);

public record MultiPortConnection(string Type, string Direction, string InternalSymbol);

public record CoverCrossTarget(string Coverpoint);

public record CoverOption(IKind Expr);

public record CoverageBinTransItem(IKind[] Items, IKind? RepeatFrom, IKind? RepeatTo, string? RepeatKind);

public record LoopDim(string Range, IKind Var);

public record CaseItem(IKind[]? Expressions, IKind? Stmt, IKind? Pattern, IKind? Filter);

public record StructurePatternField(string Field, IKind Pattern);

public record RandSeqRule(IKind[] Prods, IKind? WeightExpr, bool? IsRandJoin);

public record WaitOrderEvent(IKind Target);

public record SequenceConcatElement(IKind Sequence, int Min, int? Max);

public record ProceduralCheckerInstance(string Instance);

public record AssertionInstanceLocalVar(string Name, IKind Value);

public record SvRoot : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvCompilationUnit : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvPackage : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public IKind[]? Attributes { get; init; }
}

public record SvAttribute : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Value { get; init; }
}

public record SvParameter : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public IKind? Initializer { get; init; }
    public string? Value { get; init; }
    public bool IsLocal { get; init; }
    public bool IsPort { get; init; }
    public bool IsBody { get; init; }
}

public record SvIntegerLiteral : IKind
{
    public string? Type { get; init; }
    public string? Value { get; init; }
    public string? Constant { get; init; }
}

public record SvTypeParameter : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public bool IsLocal { get; init; }
    public bool IsPort { get; init; }
    public bool IsBody { get; init; }
}

public record SvAnonymousProgram : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPrimitive : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public bool IsSequential { get; init; }
    public PrimitiveTableEntry[]? Table { get; init; }
}

public record SvPrimitivePort : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Direction { get; init; }
}

public record SvConfigBlock : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvConversion : IKind
{
    public string? Type { get; init; }
    public IKind? Operand { get; init; }
    public string? Constant { get; init; }
}

public record SvEnumType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public string? BaseType { get; init; }
}

public record SvEnumValue : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Value { get; init; }
}

public record SvTypeAlias : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Target { get; init; }
}

public record SvChecker : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvAssertionPort : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Direction { get; init; }
}

public record SvClassType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public bool IsAbstract { get; init; }
    public bool IsInterface { get; init; }
    public bool IsFinal { get; init; }
    public string? BaseClass { get; init; }
    public IKind[]? Implements { get; init; }
    public string? GenericClass { get; init; }
}

public record SvClassProperty : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Lifetime { get; init; }
    public string? Visibility { get; init; }
    public string? RandMode { get; init; }
    public IKind? Initializer { get; init; }
    public string? Flags { get; init; }
}

public record SvMethodPrototype : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public string? ReturnType { get; init; }
    public string? SubroutineKind { get; init; }
    public string? Visibility { get; init; }
    public IKind[]? Arguments { get; init; }
    public string? Flags { get; init; }
    public IKind? Subroutine { get; init; }
}

public record SvFormalArgument : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Lifetime { get; init; }
    public string? Direction { get; init; }
    public IKind? DefaultValue { get; init; }
    public string? Flags { get; init; }
}

public record SvSubroutine : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public string? ReturnType { get; init; }
    public string? DefaultLifetime { get; init; }
    public string? SubroutineKind { get; init; }
    public IKind? Body { get; init; }
    public string? Visibility { get; init; }
    public IKind[]? Arguments { get; init; }
    public string? Flags { get; init; }
}

public record SvVariable : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Lifetime { get; init; }
    public string? Flags { get; init; }
    public IKind? Initializer { get; init; }
}

public record SvExpressionStatement : IKind
{
    public IKind? Expr { get; init; }
}

public record SvAssignment : IKind
{
    public string? Type { get; init; }
    public IKind? Left { get; init; }
    public IKind? Right { get; init; }
    public bool IsNonBlocking { get; init; }
    public string? Op { get; init; }
    public IKind? TimingControl { get; init; }
}

public record SvNamedValue : IKind
{
    public string? Type { get; init; }
    public string? Symbol { get; init; }
    public string? Constant { get; init; }
}

public record SvBinaryOp : IKind
{
    public string? Type { get; init; }
    public SvBinaryOperator Op { get; init; }
    public IKind? Left { get; init; }
    public IKind? Right { get; init; }
    public string? Constant { get; init; }
}

public record SvList : IKind
{
    public IKind[]? List { get; init; }
}

public record SvGenericClassDef : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Specializations { get; init; }
}

public record SvReturn : IKind
{
    public IKind? Expr { get; init; }
}

public record SvCall : IKind
{
    public string? Type { get; init; }
    public string? Subroutine { get; init; }
    public IKind[]? Arguments { get; init; }
    public IKind? ThisClass { get; init; }
}

public record SvConstraintBlock : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public IKind? Constraints { get; init; }
}

public record SvExpression : IKind
{
    public IKind? Expr { get; init; }
    public bool IsSoft { get; init; }
}

public record SvDist : IKind
{
    public string? Type { get; init; }
    public IKind? Left { get; init; }
    public IKind[]? Items { get; init; }
}

public record SvPerRange : IKind
{
    public IKind? Value { get; init; }
    public IKind? Weight { get; init; }
}

public record SvPerValue : IKind
{
    public IKind? Value { get; init; }
    public IKind? Weight { get; init; }
}

public record SvElementSelect : IKind
{
    public string? Type { get; init; }
    public IKind? Value { get; init; }
    public IKind? Selector { get; init; }
}

public record SvInside : IKind
{
    public string? Type { get; init; }
    public IKind? Left { get; init; }
    public IKind[]? RangeList { get; init; }
}

public record SvRangeSelect : IKind
{
    public string? Type { get; init; }
    public string? SelectionKind { get; init; }
    public IKind? Value { get; init; }
    public IKind? Left { get; init; }
    public IKind? Right { get; init; }
}

public record SvSolveBefore : IKind
{
    public IKind[]? Solve { get; init; }
    public IKind[]? After { get; init; }
}

public record SvImplication : IKind
{
    public IKind? Predicate { get; init; }
    public IKind? Body { get; init; }
}

public record SvConditional : IKind
{
    public IKind? Predicate { get; init; }
    public IKind? IfBody { get; init; }
    public IKind? ElseBody { get; init; }
    public SvExpressionStatement[]? Conditions { get; init; }
    public string? Check { get; init; }
    public IKind? IfTrue { get; init; }
    public IKind? IfFalse { get; init; }
    public IKind? Condition { get; init; }
    public IKind? If { get; init; }
    public IKind? Else { get; init; }
}

public record SvForeach : IKind
{
    public IKind? ArrayRef { get; init; }
    public LoopDim[]? LoopDims { get; init; }
    public IKind? Body { get; init; }
}

public record SvIterator : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
}

public record SvDisableSoft : IKind
{
    public IKind? Target { get; init; }
}

public record SvCovergroupType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    [JsonPropertyName("event")] public IKind? EventObj { get; init; }
}

public record SvCovergroupBody : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public CoverOption[]? Options { get; init; }
}

public record SvCoverpoint : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public CoverOption[]? Options { get; init; }
    public IKind? Iff { get; init; }
}

public record SvCoverageBin : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? BinsKind { get; init; }
    public bool IsArray { get; init; }
    public bool IsWildcard { get; init; }
    public bool IsDefault { get; init; }
    public bool IsDefaultSequence { get; init; }
    public IKind[]? Values { get; init; }
    public CoverageBinTransItem[][]? Trans { get; init; }
    public IKind? CrossSelect { get; init; }
}

public record SvValueRange : IKind
{
    public string? Type { get; init; }
    public IKind? Left { get; init; }
    public IKind? Right { get; init; }
    public string? RangeKind { get; init; }
}

public record SvUnboundedLiteral : IKind
{
    public string? Type { get; init; }
}

public record SvCoverCross : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public CoverCrossTarget[]? Targets { get; init; }
    public CoverOption[]? Options { get; init; }
}

public record SvCoverCrossBody : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvSetExpr : IKind
{
    public IKind? Expr { get; init; }
}

public record SvSimpleAssignmentPattern : IKind
{
    public string? Type { get; init; }
    public IKind[]? Elements { get; init; }
}

public record SvWithFilter : IKind
{
    public IKind? Expr { get; init; }
    public IKind? Filter { get; init; }
}

public record SvBinary : IKind
{
    public IKind? Left { get; init; }
    public IKind? Right { get; init; }
    public string? Op { get; init; }
}

public record SvUnary : IKind
{
    public IKind? Expr { get; init; }
    public string? Op { get; init; }
}

public record SvCondition : IKind
{
    public string? Target { get; init; }
    public IKind[]? Intersects { get; init; }
}

public record SvSignalEvent : IKind
{
    public IKind? Expr { get; init; }
    public SvEdgeKind? Edge { get; init; }
}

public record SvInstance : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind? Body { get; init; }
    public InstanceConnection[]? Connections { get; init; }
}

public record SvInstanceBody : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public string? Definition { get; init; }
}

public record SvDefinition : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? DefaultNetType { get; init; }
    public string? DefinitionKind { get; init; }
    public string? DefaultLifeTime { get; init; }
    public string? UnconnectedDrive { get; init; }
    public bool? CellDefine { get; init; }
}

public record SvPort : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Direction { get; init; }
    public string? InternalSymbol { get; init; }
    public bool IsNullPort { get; init; }
    public IKind? Initializer { get; init; }
    public IKind[]? Attributes { get; init; }
}

public record SvNet : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public IKind? NetType { get; init; }
    public IKind? Initializer { get; init; }
    public bool IsImplicit { get; init; }
}

public record SvNetType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
}

public record SvSpecifyBlock : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvSpecparam : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public IKind? Initializer { get; init; }
    public bool IsPathPulse { get; init; }
    public string? Value { get; init; }
}

public record SvTimingPath : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? ConnectionKind { get; init; }
    public string? Polarity { get; init; }
    public string? EdgePolarity { get; init; }
    public string? EdgeIdentifier { get; init; }
    public bool IsStateDependent { get; init; }
    public IKind? ConditionExpr { get; init; }
    public IKind[]? Inputs { get; init; }
    public IKind[]? Outputs { get; init; }
    public IKind[]? Delays { get; init; }
}

public record SvRealLiteral : IKind
{
    public string? Type { get; init; }
    public double Value { get; init; }
    public string? Constant { get; init; }
}

public record SvStatementBlock : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvProceduralBlock : IKind
{
    public long Addr { get; init; }
    public SvProceduralBlockKind? ProcedureKind { get; init; }
    public IKind? Body { get; init; }
}

public record SvBlock : IKind
{
    public string? BlockKind { get; init; }
    public string? Block { get; init; }
    public IKind? Body { get; init; }
}

public record SvVariableDeclaration : IKind
{
    public string? Symbol { get; init; }
}

public record SvStreaming : IKind
{
    public string? Type { get; init; }
    public int SliceSize { get; init; }
    public IKind[]? Streams { get; init; }
}

public record SvMemberAccess : IKind
{
    public string? Type { get; init; }
    public string? Member { get; init; }
    public IKind? Value { get; init; }
}

public record SvNewClass : IKind
{
    public string? Type { get; init; }
    public bool IsSuperClass { get; init; }
    public IKind? SizeExpr { get; init; }
}

public record SvModport : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvModportPort : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Direction { get; init; }
    public string? InternalSymbol { get; init; }
}

public record SvInterfacePort : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? InterfaceDef { get; init; }
    public string? Modport { get; init; }
    public bool IsGeneric { get; init; }
}

public record SvWildcardImport : IKind
{
    public long Addr { get; init; }
    public bool IsFromExport { get; init; }
    public string? Package { get; init; }
}

public record SvMultiPort : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public string? Direction { get; init; }
    public MultiPortConnection[]? Ports { get; init; }
}

public record SvUnbasedUnsizedIntegerLiteral : IKind
{
    public string? Type { get; init; }
    public string? Value { get; init; }
    public string? Constant { get; init; }
}

public record SvElabSystemTask : IKind
{
    public long Addr { get; init; }
    public string? TaskKind { get; init; }
    public string? Message { get; init; }
}

public record SvContinuousAssign : IKind
{
    public long Addr { get; init; }
    public IKind? Assignment { get; init; }
    public IKind? Delay { get; init; }
    public string? DriveStrength0 { get; init; }
    public string? DriveStrength1 { get; init; }
}

public record SvDelay3 : IKind
{
    public IKind? Expr1 { get; init; }
    public IKind? Expr2 { get; init; }
}

public record SvMinTypMax : IKind
{
    public string? Type { get; init; }
    public IKind? Selected { get; init; }
}

public record SvNetAlias : IKind
{
    public long Addr { get; init; }
    public IKind[]? NetReferences { get; init; }
}

public record SvConcatenation : IKind
{
    public string? Type { get; init; }
    public IKind[]? Operands { get; init; }
    public string? Constant { get; init; }
}

public record SvRepeatLoop : IKind
{
    public IKind? Count { get; init; }
    public IKind? Body { get; init; }
}

public record SvTimed : IKind
{
    public IKind? Timing { get; init; }
    public IKind? Stmt { get; init; }
}

public record SvDelay : IKind
{
    public IKind? Expr { get; init; }
}

public record SvWait : IKind
{
    public IKind? Cond { get; init; }
    public IKind? Stmt { get; init; }
}

public record SvUnaryOp : IKind
{
    public string? Type { get; init; }
    public SvUnaryOperator? Op { get; init; }
    public IKind? Operand { get; init; }
}

public record SvWaitFork : IKind
{
}

public record SvWaitOrder : IKind
{
    public WaitOrderEvent[]? Events { get; init; }
    public IKind? IfTrue { get; init; }
}

public record SvHierarchicalValue : IKind
{
    public string? Type { get; init; }
    public string? Symbol { get; init; }
}

public record SvDisable : IKind
{
    public IKind? Target { get; init; }
}

public record SvArbitrarySymbol : IKind
{
    public string? Type { get; init; }
    public string? Symbol { get; init; }
}

public record SvProceduralAssign : IKind
{
    public IKind? Assignment { get; init; }
    public bool IsForce { get; init; }
}

public record SvProceduralDeassign : IKind
{
    public IKind? Lvalue { get; init; }
    public bool IsRelease { get; init; }
}

public record SvCase : IKind
{
    public string? Condition { get; init; }
    public string? Check { get; init; }
    public IKind? Expr { get; init; }
    public CaseItem[]? Items { get; init; }
    public IKind? DefaultCase { get; init; }
}

public record SvEmpty : IKind
{
}

public record SvForeverLoop : IKind
{
    public IKind? Body { get; init; }
}

public record SvBreak : IKind
{
}

public record SvContinue : IKind
{
}

public record SvWhileLoop : IKind
{
    public IKind? Cond { get; init; }
    public IKind? Body { get; init; }
}

public record SvForLoop : IKind
{
    public IKind[]? Initializers { get; init; }
    public IKind? StopExpr { get; init; }
    public IKind[]? Steps { get; init; }
    public IKind? Body { get; init; }
}

public record SvLValueReference : IKind
{
    public string? Type { get; init; }
}

public record SvForeachLoop : IKind
{
    public IKind? ArrayRef { get; init; }
    public LoopDim[]? LoopDims { get; init; }
    public IKind? Body { get; init; }
}

public record SvStringLiteral : IKind
{
    public string? Type { get; init; }
    public string? Literal { get; init; }
}

public record SvPatternCase : IKind
{
    public string? Condition { get; init; }
    public string? Check { get; init; }
    public IKind? Expr { get; init; }
    public CaseItem[]? Items { get; init; }
}

public record SvTagged : IKind
{
    public string? Member { get; init; }
    public IKind? ValuePattern { get; init; }
}

public record SvStructure : IKind
{
    public StructurePatternField[]? Patterns { get; init; }
}

public record SvWildcard : IKind
{
}

public record SvConstant : IKind
{
    public IKind? Expr { get; init; }
}

public record SvConditionalOp : IKind
{
    public string? Type { get; init; }
    public CaseItem[]? Conditions { get; init; }
    public IKind? Left { get; init; }
    public IKind? Right { get; init; }
}

public record SvGenvar : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvGenerateBlockArray : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public int ConstructIndex { get; init; }
}

public record SvGenerateBlock : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public int ConstructIndex { get; init; }
    public bool IsUninstantiated { get; init; }
}

public record SvEmptyMember : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvImmediateAssertion : IKind
{
    public IKind? Cond { get; init; }
    public IKind? IfFalse { get; init; }
    public string? AssertionKind { get; init; }
    public bool IsDeferred { get; init; }
    public bool IsFinal { get; init; }
}

public record SvProperty : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvConcurrentAssertion : IKind
{
    public IKind? PropertySpec { get; init; }
    public IKind? IfTrue { get; init; }
    public IKind? IfFalse { get; init; }
    public string? AssertionKind { get; init; }
}

public record SvSimple : IKind
{
    public IKind? Expr { get; init; }
}

public record SvAssertionInstance : IKind
{
    public string? Type { get; init; }
    public string? Symbol { get; init; }
    public IKind? Body { get; init; }
    public bool IsRecursiveProperty { get; init; }
    public AssertionInstanceLocalVar[]? LocalVars { get; init; }
}

public record SvClocking : IKind
{
    public IKind? ClockingObj { get; init; }
    public IKind? Expr { get; init; }
}

public record SvSequenceConcat : IKind
{
    public SequenceConcatElement[]? Elements { get; init; }
}

public record SvPrimitiveInstance : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? PrimitiveType { get; init; }
    public IKind[]? Ports { get; init; }
    public IKind? Delay { get; init; }
}

public record SvOneStepDelay : IKind
{
}

public record SvDefParam : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Target { get; init; }
    public string? Value { get; init; }
}

public record SvClockingBlock : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    [JsonPropertyName("event")] public IKind? EventObj { get; init; }
    public SvClockingSkew? DefaultInputSkew { get; init; }
    public SvClockingSkew? DefaultOutputSkew { get; init; }
}

public record SvClockingSkew
{
    public IKind? Delay { get; init; }
    public SvEdgeKind? Edge { get; init; }
}

public record SvClockVar : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
    public IKind? Initializer { get; init; }
    public string? Lifetime { get; init; }
    public string? Direction { get; init; }
    public SvClockingSkew? Skew { get; init; }
}

public record SvEventList : IKind
{
    public IKind[]? Events { get; init; }
}

public record SvCheckerInstance : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind? Body { get; init; }
    public InstanceConnection[]? Connections { get; init; }
}

public record SvCheckerInstanceBody : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public string? Checker { get; init; }
    public bool IsProcedural { get; init; }
}

public record SvProceduralChecker : IKind
{
    public ProceduralCheckerInstance[]? Instances { get; init; }
}

public record SvSequence : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvRandSeqProduction : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
    public string? ReturnType { get; init; }
    public IKind[]? Arguments { get; init; }
    public RandSeqRule[]? Rules { get; init; }
}

public record SvItem : IKind
{
    public IKind? Item { get; init; }
    public string? Target { get; init; }
    public IKind[]? Args { get; init; }
}

public record SvRepeat : IKind
{
    public IKind? Expr { get; init; }
    public IKind? Item { get; init; }
}

public record SvIfElse : IKind
{
    public IKind? Expr { get; init; }
    public IKind? IfItem { get; init; }
    public IKind? ElseItem { get; init; }
}

public record SvCodeBlock : IKind
{
}

public record SvEmptyArgument : IKind
{
    public required string Type { get; init; }
}

public record SvNewArray : IKind
{
    public string? Type { get; init; }
    public IKind? SizeExpr { get; init; }
    public IKind? Initializer { get; init; }
}

public record SvNewCovergroup : IKind
{
    public string? Type { get; init; }
    public IKind[]? Arguments { get; init; }
}

public record SvCopyClass : IKind
{
    public string? Type { get; init; }
    public IKind? SourceExpr { get; init; }
}

public record SvTimeLiteral : IKind
{
    public string? Type { get; init; }
    public double Value { get; init; }
    public string? Constant { get; init; }
}

public record SvNullLiteral : IKind
{
    public string? Type { get; init; }
}

public record SvReplication : IKind
{
    public string? Type { get; init; }
    public IKind? Count { get; init; }
    public IKind? Expr { get; init; }
}

public record SvDataType : IKind
{
    public string? Type { get; init; }
    public string? TargetType { get; init; }
}

public record SvTypeReference : IKind
{
    public string? Type { get; init; }
    public IKind? TargetExpr { get; init; }
}

public record SvStructuredAssignmentPattern : IKind
{
    public string? Type { get; init; }
    public IKind[]? Elements { get; init; }
}

public record SvReplicatedAssignmentPattern : IKind
{
    public string? Type { get; init; }
    public IKind? Count { get; init; }
    public IKind[]? Elements { get; init; }
}

public record SvClockingEvent : IKind
{
    public string? Type { get; init; }
}

public record SvTaggedUnion : IKind
{
    public string? Type { get; init; }
    public string? Member { get; init; }
    public IKind? Value { get; init; }
}

public record SvDoWhileLoop : IKind
{
    public IKind? Cond { get; init; }
    public IKind? Body { get; init; }
}

public record SvDisableFork : IKind
{
}

public record SvEventTrigger : IKind
{
    public IKind? Target { get; init; }
    public bool IsNonBlocking { get; init; }
}

public record SvRandCase : IKind
{
    public CaseItem[]? Items { get; init; }
}

public record SvRandSequence : IKind
{
    public IKind[]? Productions { get; init; }
}

public record SvDeferredMember : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvTransparentMember : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPredefinedIntegerType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvScalarType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvFloatingType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPackedArrayType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvFixedSizeUnpackedArrayType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvDynamicArrayType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvDpiOpenArrayType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvAssociativeArrayType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvQueueType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPackedStructType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvUnpackedStructType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPackedUnionType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvUnpackedUnionType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvVoidType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvNullType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvCHandleType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvStringType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvEventType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvUnboundedType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvTypeRefType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvUntypedType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvSequenceType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPropertyType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvVirtualInterfaceType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvErrorType : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvForwardingTypedef : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvModportClocking : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Members { get; init; }
}

public record SvInstanceArray : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public IKind[]? Instances { get; init; }
}

public record SvExplicitImport : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Package { get; init; }
}

public record SvField : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
    public string? Type { get; init; }
}

public record SvUninstantiatedDef : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPatternVar : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvLocalAssertionVar : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvLetDecl : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvPulseStyle : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvSystemTimingCheck : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}

public record SvUniqueness : IKind
{
    public IKind[]? Expressions { get; init; }
}

public record SvSequenceWithMatch : IKind
{
    public IKind? Expr { get; init; }
    public IKind[]? Match { get; init; }
}

public record SvFirstMatch : IKind
{
    public IKind? Expr { get; init; }
}

public record SvStrongWeak : IKind
{
    public IKind? Expr { get; init; }
    public bool IsStrong { get; init; }
}

public record SvAbort : IKind
{
    public IKind? Expr { get; init; }
    public bool IsAcceptOn { get; init; }
}

public record SvDisableIff : IKind
{
    public IKind? Cond { get; init; }
    public IKind? Expr { get; init; }
}

public record SvCrossId : IKind
{
    public string? Coverpoint { get; init; }
}

public record SvTempVar : IKind
{
    public string? Name { get; init; }
    public long Addr { get; init; }
}