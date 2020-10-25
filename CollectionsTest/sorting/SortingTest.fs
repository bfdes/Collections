namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<AbstractClass>]
type SortingTest<'t when 't : comparison>() = 
    member this.maxSize = 10
    
    abstract member Sort: array<'t> -> unit

    abstract member Arrays: seq<array<'t>>

    static member IsSorted (array: array<'u>) =
        array 
        |> Seq.pairwise
        |> Seq.forall(fun (l, r) -> l <= r) 

    [<TestMethod>]
    member this.ItSortsArray() =
        let test array =
            this.Sort array
            SortingTest<'t>.IsSorted array

        Assert.All test this.Arrays

    [<TestMethod>]
    member this.ItRearrangesKeys() =
        let test array = 
            let before = Histogram array
            this.Sort array
            let after = Histogram array
            before = after

        Assert.All test this.Arrays

[<AbstractClass>]
type SortingTest() =
    inherit SortingTest<int>()
    member this.rnd = new Random()

    override this.Arrays =
        let sample size =
            Array.init size (fun _ -> this.rnd.Next() )

        let samples size = 
            let multiples = int(Math.Pow(3.0, float size))
            Seq.init multiples (fun _ -> sample size)

        Seq.init this.maxSize id
        |> Seq.collect samples
