namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<AbstractClass>]
type ListTest() =
    let maxSize = 20
    let rnd = Random()

    abstract member List: array<Integer> -> IList<Integer>

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
    member this.EmptyListShouldHaveSizeZero() =
        let empty = this.List Array.empty
        Assert.IsTrue(empty.IsEmpty())
        Assert.AreEqual(empty.Size(), 0)

    [<TestMethod>]
    member this.ItShouldEmptyInLIFOOrder() = 
        let test elems = 
            this.List elems
            |> ListTest.IsSorted

        let reversedArray size =
            Array.init size (fun i -> Integer(size-1-i))

        Seq.init maxSize id
        |> Seq.map reversedArray
        |> Assert.All test 

    [<TestMethod>]
    member this.CannotPopEmptyList() =
        let test elems = 
            let list = this.List elems

            while not(list.IsEmpty()) do
                list.Pop() |> ignore

            Assert.ThrowsException<InvalidOperationException>( fun () ->
                list.Pop() |> ignore
            ) |> ignore

        this.Arrays maxSize
        |> Seq.iter test

    [<TestMethod>]
    member this.PeekShouldHaveNoSideEffect() =
        let test elems = 
            let list = this.List elems

            while not(list.IsEmpty()) do
                if list.Peek() <> list.Pop() then
                    Assert.Fail()

        this.Arrays maxSize
        |> Seq.iter test
