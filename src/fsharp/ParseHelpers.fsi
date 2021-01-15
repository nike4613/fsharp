// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Pos
open FSharp.Compiler.Text.Range
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: range

exception IndentationProblem of string * range

val warningStringOfCoords: line:int -> column:int -> string

val warningStringOfPos: p:pos -> string

val posOfLexPosition: p:Position -> pos

val mkSynRange: p1:Position -> p2:Position -> range

type LexBuffer<'Char> with
    member LexemeRange: range

val lhs: parseState:IParseState -> range

val rhs2: parseState:IParseState -> i:int -> j:int -> range

val rhs: parseState:IParseState -> i:int -> range

type IParseState with
    member SynArgNameGenerator: SyntaxTreeOps.SynArgNameGenerator
    member ResetSynArgNameGenerator: unit -> unit

module LexbufLocalXmlDocStore =
    val ClearXmlDoc: lexbuf:UnicodeLexing.Lexbuf -> unit
    val SaveXmlDocLine: lexbuf:UnicodeLexing.Lexbuf * lineText:string * range:range -> unit
    val GrabXmlDocBeforeMarker: lexbuf:UnicodeLexing.Lexbuf * markerRange:range -> XmlDoc.PreXmlDoc
  
type LexerIfdefStackEntry =
    | IfDefIf
    | IfDefElse

type LexerIfdefStackEntries = (LexerIfdefStackEntry * range) list

type LexerIfdefStack = LexerIfdefStackEntries

type LexerEndlineContinuation =
    | Token
    | Skip of int * range: range

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression * LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression * LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

val LexerIfdefEval: lookup:(string -> bool) -> _arg1:LexerIfdefExpression -> bool

[<RequireQualifiedAccess>]
type LexerStringStyle =
    | Verbatim
    | TripleQuote
    | SingleQuote

[<RequireQualifiedAccess; StructAttribute>]
type LexerStringKind =
    { IsByteString: bool
      IsInterpolated: bool
      IsInterpolatedFirst: bool }
    static member ByteString: LexerStringKind
    static member InterpolatedStringFirst: LexerStringKind
    static member InterpolatedStringPart: LexerStringKind
    static member String: LexerStringKind
    
type LexerInterpolatedStringNesting =
    (int * LexerStringStyle * range) list

[<RequireQualifiedAccess; NoComparison;NoEquality>]
type LexerContinuation =
    | Token of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting
    | IfDefSkip of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | String of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * style: LexerStringStyle * kind: LexerStringKind * range: range
    | Comment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | SingleLineComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | StringInComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * style: LexerStringStyle * int * range: range
    | MLOnly of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * range: range
    | EndLine of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * LexerEndlineContinuation

    member LexerIfdefStack: LexerIfdefStackEntries

    member LexerInterpStringNesting: LexerInterpolatedStringNesting

    static member Default: LexerContinuation
    
and LexCont = LexerContinuation

val ParseAssemblyCodeInstructions: s:string -> isFeatureSupported:(Features.LanguageFeature -> bool) -> m:range -> ILInstr[]

val ParseAssemblyCodeType: s:string -> isFeatureSupported:(Features.LanguageFeature -> bool) -> m:range -> ILType

val parseSmallInt: errorLogger: ErrorLogger -> m: range -> s: string -> int

val parseInt32AllowMaxIntPlusOne: errorLogger: ErrorLogger -> m: range -> s: string -> int32 * bool

val parseInt32: errorLogger: ErrorLogger -> m: range -> s: string -> int32

val parseUInt32: errorLogger: ErrorLogger -> m: range -> s: string -> uint32

val parseInt64AllowMaxIntPlusOne: errorLogger: ErrorLogger -> m: range -> s: string -> int64 * bool

val parseInt64: errorLogger: ErrorLogger -> m: range -> s: string -> int64

val parseUInt64: errorLogger: ErrorLogger -> m: range -> s: string -> uint64

val parseNativeIntAllowMaxIntPlusOne: errorLogger: ErrorLogger -> m: range -> s: string -> int64 * bool

val parseNativeInt: errorLogger: ErrorLogger -> m: range -> s: string -> int64

val parseUNativeInt: errorLogger: ErrorLogger -> m: range -> s: string -> uint64

val convSmallIntToSByteAllowMaxIntPlusOne: errorLogger: ErrorLogger -> m: range -> n: int -> sbyte * bool

val convSmallIntToSByte: errorLogger: ErrorLogger -> m: range -> n: int -> sbyte

val convSmallIntToInt16AllowMaxIntPlusOne: errorLogger: ErrorLogger -> m: range -> n: int -> int16 * bool

val convSmallIntToInt16: errorLogger: ErrorLogger -> m: range -> n: int -> int16

val convSmallIntToByte: errorLogger: ErrorLogger -> m: range -> n: int -> byte

val convSmallIntToUInt16: errorLogger: ErrorLogger -> m: range -> n: int -> uint16

val parseDouble: errorLogger: ErrorLogger -> m: range -> s: string -> double

val parseSingle: errorLogger: ErrorLogger -> m: range -> s: string -> single

val parseDecimal: errorLogger: ErrorLogger -> m: range -> s: string -> decimal
