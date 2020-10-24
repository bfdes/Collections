namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<AbstractClass>]
type TreeTest() =
    let maxSize = 8
    let rnd = Random()

    abstract member Tree: seq<int*int> -> ITree<int, int>

    static member IsSorted seq =
        seq
        |> Seq.pairwise
        |> Seq.forall(fun (l, r) -> l <= r)

    member this.Samples = 
        let sample size =
            Array.init size (fun _ -> (rnd.Next(), rnd.Next()))

        let mutable multiples = 1

        let samples size = 
            let out = seq { for _ in 1..multiples -> sample size }
            multiples <- 1 + size * multiples
            out
            
        Seq.init maxSize id
        |> Seq.collect samples

    [<TestMethod>]
    member this.EmptyTreeShouldHaveSizeZero() =
        let empty = this.Tree Seq.empty
        Assert.IsTrue(empty.IsEmpty())
        Assert.AreEqual(empty.Size(), 0)

    [<TestMethod>]
    member this.GetTest() =
        // Property to verify: Get(k) = v forall (k, v) in tree
        let test pairs = 
            let tree = this.Tree pairs
            pairs
            |> Seq.forall (fun (k, v) -> tree.Get(k) = v)

        Assert.All test this.Samples

    [<Ignore>]
    [<TestMethod>]
    member this.DeleteTest() =
        let test pairs =
            let tree = this.Tree pairs
            pairs
            |> Seq.map fst
            |> Seq.iter tree.Delete  // Should not throw
            tree.IsEmpty()

        Assert.All test this.Samples

    [<TestMethod>]
    member this.ItShouldReturnKeysInOrder() =
        let test pairs = 
            let tree = this.Tree pairs
            tree.Keys() |> TreeTest.IsSorted

        Assert.All test this.Samples

    [<TestMethod>]
    member this.ItMaintainsSymmetricOrder() =
        let test pairs = 
            let tree = this.Tree pairs
            tree.IsSymmetric()

        Assert.All test this.Samples

    [<TestMethod>]
    member this.MinTest() = 
        let test pairs = 
            let min = pairs |> Seq.map fst |> Seq.min  // Get min by linear scan
            let tree = this.Tree pairs
            tree.Min() = min
        
        this.Samples
        |> Seq.skip 1  // Skip the empty array
        |> Assert.All test

    [<TestMethod>]
    member this.MaxTest() = 
        let test pairs = 
            let max = pairs |> Seq.map fst |> Seq.max
            let tree = this.Tree pairs
            tree.Max() = max

        this.Samples
        |> Seq.skip 1
        |> Assert.All test

    [<TestMethod>]
    member this.FloorTest () =
        // Property to verify: tree.Floor(k) = k forall k in tree.Keys()
        let test pairs = 
            let tree = this.Tree pairs
            pairs
            |> Seq.map fst
            |> Seq.forall (fun k -> tree.Floor(k) = k)

        Assert.All test this.Samples

    [<TestMethod>]
    member this.CeilingTest () =
        let test pairs = 
            let tree = this.Tree pairs
            pairs
            |> Seq.map fst
            |> Seq.forall (fun k -> tree.Ceiling(k) = k)

        Assert.All test this.Samples

    [<TestMethod>]
    member this.RankTest() =
        // n.b. This property verifies that the keys of the tree are correctly ranked, 
        // it does not check that the Rank method returns the correct value for ALL arguments.
        let test pairs = 
            let tree = this.Tree pairs
            pairs
            |> Seq.map fst
            |> Seq.distinct
            |> Seq.sort
            |> Seq.mapi (fun i k -> (i, k))
            |> Seq.forall (fun (i, k) -> tree.Rank(k) = i)

        Assert.All test this.Samples

    [<TestMethod>]
    member this.FloorAgreesWithMax() =
        // Property to verify: tree.Floor(Int.Max) = tree.Max()  
        let test pairs = 
            let tree = this.Tree pairs
            tree.Floor(Int32.MaxValue) = tree.Max()

        this.Samples
        |> Seq.skip 1
        |> Assert.All test

    [<TestMethod>]
    member this.CeilingAgreesWithMin() = 
        let test pairs = 
            let tree = this.Tree pairs
            tree.Ceiling(Int32.MinValue) = tree.Min()

        this.Samples
        |> Seq.skip 1
        |> Assert.All test
