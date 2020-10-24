namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<AbstractClass>]
type QueueTest() =
    let rnd = Random()
    member this.maxSize = 20

    abstract member Queue: array<Integer> -> IQueue<Integer>

    static member IsSorted seq =
        seq
        |> Seq.pairwise
        |> Seq.forall(fun (l, r) -> l <= r)

    member this.Arrays maxSize = 
        let array size = 
            Array.init size (fun _ -> Integer(rnd.Next()))

        Seq.init maxSize id
        |> Seq.map array

    [<TestMethod>]
    member this.EmptyQueueShouldHaveSizeZero() =
        let empty = this.Queue Array.empty
        Assert.IsTrue(empty.IsEmpty())
        Assert.AreEqual(empty.Size(), 0)

    [<TestMethod>]
    member this.ItShouldEmptyInFIFOOrder() = 
        let test elems = 
            this.Queue elems
            |> QueueTest.IsSorted

        let array size =
            Array.init size Integer

        Seq.init this.maxSize id
        |> Seq.map array
        |> Assert.All test 

    [<TestMethod>]
    member this.CannotPopEmptyQueue() =
        let test elems = 
            let queue = this.Queue elems

            while not(queue. IsEmpty()) do
                queue.Pop() |> ignore

            Assert.ThrowsException<InvalidOperationException>( fun () ->
                queue.Pop() |> ignore
            ) |> ignore

        this.Arrays this.maxSize
        |> Seq.iter test

    [<TestMethod>]
    member this.PeekShouldHaveNoSideEffect() =
        let test elems = 
            let queue = this.Queue elems

            while not(queue.IsEmpty()) do
                if queue.Peek() <> queue.Pop() then
                    Assert.Fail()

        this.Arrays this.maxSize
        |> Seq.iter test
