namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type ArrayQueueTest() =
    inherit QueueTest()

    override this.Queue array =
        upcast ResizingBuffer array

    [<TestMethod; ExpectedException(typeof<ArgumentOutOfRangeException>)>]
    member this.InitialCapacityMustBeFinite() =
        ResizingBuffer<Integer>(0) |> ignore
