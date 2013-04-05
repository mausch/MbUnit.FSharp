module MbUnit.FSharp

open System
open MbUnit.Framework

let testList name tests = 
    let suite = TestSuite name
    Seq.iter suite.Children.Add tests
    suite :> Test

let testCase name (test: unit -> unit) = 
    TestCase(name, Gallio.Common.Action test) :> Test

let testFixture setup = 
    Seq.map (fun (name, partialTest) ->
                testCase name (setup partialTest))

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
