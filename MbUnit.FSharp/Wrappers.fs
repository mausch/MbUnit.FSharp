module MbUnit.FSharp

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

type TestCaseBuilder(name: string) = 
    member x.Zero() = ()
    member x.Delay f = f
    member x.Run f = testCase name f

let inline test name = TestCaseBuilder name
