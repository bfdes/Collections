namespace CollectionsTest

open System 
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type BinaryHeapTest() =
    let rnd = Random()
    let maxSize = 7
    
    static member IsSorted seq =
        seq
        |> Seq.pairwise
        |> Seq.forall(fun (l, r) -> l <= r)

    member this.Queue keys =
        let queue = BinaryHeap<Integer>()
        Seq.iter queue.Push keys
        queue
    
    member this.Arrays maxSize = 
        let array size = 
            Array.init size (fun _ -> Integer(rnd.Next()))

        let samples size = 
            let multiples = int(Math.Pow(3.0, float size))
            Seq.init multiples (fun _ -> array size)
    
        Seq.init maxSize (fun i -> i+1)
        |> Seq.collect samples

    [<TestMethod; ExpectedException(typeof<ArgumentOutOfRangeException>)>]
    member this.InitialCapacityMustBeFinite() =
        BinaryHeap<Integer>(0) |> ignore

    [<TestMethod>]
    member this.EmptyQueueShouldHaveSizeZero() =
        let empty = this.Queue Array.empty
        Assert.IsTrue(empty.IsEmpty())
        Assert.AreEqual(empty.Size(), 0)
    
    [<TestMethod>]
    member this.ItShouldEmptyInOrder() = 
        let test keys = 
            this.Queue keys
            |> Seq.unfold (fun q -> if q.IsEmpty() then None else Some(q.Pop(), q))
            |> BinaryHeapTest.IsSorted
            
        let array size =
            Array.init size Integer

        Seq.init maxSize id
        |> Seq.map array
        |> Assert.All test 

    [<TestMethod>]
    member this.CannotPopEmptyQueue() =
        let test keys = 
            let queue = this.Queue keys

            while not(queue. IsEmpty()) do
                queue.Pop() |> ignore

            Assert.ThrowsException<InvalidOperationException>( fun () ->
                queue.Pop() |> ignore
            ) |> ignore

        this.Arrays maxSize
        |> Seq.iter test

    [<TestMethod>]
    member this.PeekShouldHaveNoSideEffect() =
        let test keys = 
            let queue = this.Queue keys

            while not(queue.IsEmpty()) do
                if queue.Peek() <> queue.Pop() then
                    Assert.Fail()

        this.Arrays maxSize
        |> Seq.iter test


    [<TestMethod>]
    member this.ItMaintainsHeapInvariant() =
        let test keys = 
            let queue = this.Queue keys
            queue.IsHeapOrdered()

        this.Arrays maxSize
        |> Assert.All test 
