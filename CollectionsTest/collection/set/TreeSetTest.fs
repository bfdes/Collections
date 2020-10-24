namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type TreeSetTest() =
    let maxSize = 8
    let rnd = Random()

    member this.Set keys =
        let set = TreeSet()
        Seq.iter set.Add keys
        set

    static member IsSorted seq =
        seq
        |> Seq.pairwise
        |> Seq.forall(fun (l, r) -> l <= r)

    member this.Samples = 
        let sample size =
            Array.init size (fun _ -> rnd.Next())

        let mutable multiples = 1

        let samples size = 
            let out = seq { for _ in 1..multiples -> sample size }
            multiples <- 1 + size * multiples
            out
            
        Seq.init maxSize id
        |> Seq.collect samples

    [<TestMethod>]
    member this.EmptySetShouldHaveSizeZero() =
        let empty = this.Set Seq.empty<int>
        Assert.IsTrue(empty.IsEmpty())
        Assert.AreEqual(empty.Size(), 0)

    [<TestMethod>]
    member this.ContainsTest() =
        let test keys = 
            let set = this.Set keys
            keys
            |> Seq.forall set.Contains

        Assert.All test this.Samples

    [<Ignore>]
    [<TestMethod>]
    member this.RemoveTest() =
        let test keys =
            let set = this.Set keys
            keys
            |> Seq.iter set.Remove  // Should not throw
            set.IsEmpty()

        Assert.All test this.Samples

    [<TestMethod>]
    member this.ItShouldReturnKeysInOrder() =
        let test keys = 
            let set = this.Set keys
            set |> TreeSetTest.IsSorted

        Assert.All test this.Samples

    [<TestMethod>]
    member this.ItMaintainsSymmetricOrder() =
        let test keys = 
            let set = this.Set keys
            set.IsSymmetric()

        Assert.All test this.Samples

    [<TestMethod>]
    member this.MinTest() = 
        let test keys = 
            let min = keys |> Seq.min
            let set = this.Set keys
            set.Min() = min
        
        this.Samples
        |> Seq.skip 1
        |> Assert.All test

    [<TestMethod>]
    member this.MaxTest() = 
        let test keys = 
            let max = keys |> Seq.max
            let set = this.Set keys
            set.Max() = max

        this.Samples
        |> Seq.skip 1
        |> Assert.All test

    [<TestMethod>]
    member this.FloorTest () =
        let test keys = 
            let set = this.Set keys
            keys
            |> Seq.forall (fun k -> set.Floor(k) = k)

        Assert.All test this.Samples

    [<TestMethod>]
    member this.CeilingTest () =
        let test keys = 
            let set = this.Set keys
            keys
            |> Seq.forall (fun k -> set.Ceiling(k) = k)

        Assert.All test this.Samples

    [<TestMethod>]
    member this.RankTest() =
        let test keys = 
            let set = this.Set keys
            keys
            |> Seq.distinct
            |> Seq.sort
            |> Seq.mapi (fun i k -> (i, k))
            |> Seq.forall (fun (i, k) -> set.Rank(k) = i)

        Assert.All test this.Samples

    [<TestMethod>]
    member this.FloorAgreesWithMax() =
        let test keys = 
            let set = this.Set keys
            set.Floor(Int32.MaxValue) = set.Max()

        this.Samples
        |> Seq.skip 1
        |> Assert.All test

    [<TestMethod>]
    member this.CeilingAgreesWithMin() = 
        let test keys = 
            let set = this.Set keys
            set.Ceiling(Int32.MinValue) = set.Min()

        this.Samples
        |> Seq.skip 1
        |> Assert.All test
