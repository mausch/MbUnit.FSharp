module MbUnit.FSharp

open System
open MbUnit.Framework
open Gallio.Model
open Gallio.Common.Reflection
open Gallio.Model.Contexts

let testList name tests = 
    let suite = TestSuite name
    Seq.iter suite.Children.Add tests
    suite :> Test

let testCase name (test: unit -> unit) = 
    TestCase(name, Gallio.Common.Action test) :> Test

let testFixture setup = 
    Seq.map (fun (name, partialTest) ->
                testCase name (setup partialTest))

let inline skiptest msg = Assert.TerminateSilently(TestOutcome.Ignored, msg)

let inline skiptestf fmt = Printf.ksprintf skiptest fmt

let inline failtest msg = Assert.Fail(msg)

let inline failtestf fmt = Printf.ksprintf failtest fmt

type TestCaseBuilder(name) = 
    member x.TryFinally(f, compensation) = 
        try
            f()
        finally
            compensation()
    member x.TryWith(f, catchHandler) = 
        try
            f()
        with e -> catchHandler e
    member x.Using(disposable: #IDisposable, f) =
        try
            f disposable
        finally
            match disposable with
            | null -> () 
            | disp -> disp.Dispose()
    member x.For(sequence, f) = 
        for i in sequence do f i
    member x.Combine(f1, f2) = f1
    member x.Zero() = ()
    member x.Delay f = f
    member x.Run f = testCase name f

let inline test name = TestCaseBuilder name

/// Run tests.
/// This will only work with a custom build of Gallio.
/// See https://github.com/mausch/MbUnit.FSharp/issues/1 for details
let run tests = 
    let codeElemInfo = 
        { new ICodeElementInfo with
            member x.Name = "F# test" 
            member x.Kind = CodeElementKind.Method
            member x.CodeReference = CodeReference.Unknown
            member x.GetAttributeInfos(attributeType, inheritt) = Seq.empty
            member x.HasAttribute(attributeType, inheritt) = false
            member x.GetAttributes(attributeType, inheritt) = Seq.empty
            member x.GetXmlDocumentation() = ""
            member x.GetCodeLocation() = CodeLocation.Unknown
            member x.ReflectionPolicy = null
            member x.Equals y = true }

    let noAction = Gallio.Common.Action ignore

    Test.RunDynamicTests(tests, codeElemInfo, noAction, noAction)
