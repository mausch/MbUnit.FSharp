module MbUnit.FSharp.Sample

open MbUnit.Framework
open MbUnit.FSharp

[<StaticTestFactory>]
let testFactory() =
    [
        testList "a sample test" [
            testList "you can nest test lists as much as you want" [
                testCase "hello world" <| fun _ -> 
                    Assert.AreEqual(4, 2+2)

                test "another, equivalent way" {
                    Assert.AreEqual(4, 2+2)
                }
            ]
        ]

        testList "some parameterized tests" [
            let data = [
                2m, 4m, 0.5m
                3m, 5m, 0.6m
            ]

            for a,b,r in data ->
                test (sprintf "%M / %M = %M" a b r) {
                    Assert.AreEqual(r, a / b)
                }
        ]

        testList "setup/teardown (a simple higher-order function)" [
            let setup f () = 
                use ms = new System.IO.MemoryStream()
                f ms

            yield! testFixture setup [
                "count", 
                    fun ms -> Assert.AreEqual(0L, ms.Length)
                "can read", 
                    fun ms -> Assert.IsTrue ms.CanRead
            ]
        ]

        testList "fixture setup" [
            // shared value between tests in this list
            // you probably shouldn't mutate this (it kills parallelization), but hey, it's possible.
            let sb = lazy System.Text.StringBuilder()

            yield testCase "add stuff" <| fun _ ->
                sb.Value.Append "hello" |> ignore

            yield test "add more stuff" {
                sb.Value.Append " world" |> ignore
            }

            yield testCase "check" <| fun _ ->
                Assert.AreEqual("hello world", sb.Value.ToString())
        ]
    ]

// this will only work with a custom build of Gallio
// see https://github.com/mausch/MbUnit.FSharp/issues/1 for details
let r = run (testFactory())
printfn "%A" r