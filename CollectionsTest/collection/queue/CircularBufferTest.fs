namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type CircularBufferTest() =
    inherit QueueTest()

    override this.Queue array =
        upcast CircularBuffer array

    [<TestMethod; ExpectedException(typeof<ArgumentOutOfRangeException>)>]
    member this.CapacityMustBeFinite() =
        CircularBuffer<Integer>(0) |> ignore

    [<TestMethod>]
    member this.CannotOverfillBuffer() =
        let test (elems: Integer[]) = 
            let queue = CircularBuffer elems
            queue.IsFull()

        this.Arrays(this.maxSize)
        |> Seq.filter (fun array -> array.Length > 0)
        |> Assert.All test
