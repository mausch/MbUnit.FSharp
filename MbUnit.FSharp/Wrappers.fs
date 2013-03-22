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

let testParam param =
    Seq.map (fun (name, partialTest) ->
                testCase name (partialTest param))
