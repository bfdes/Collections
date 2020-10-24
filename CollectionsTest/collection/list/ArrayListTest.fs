namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type ArrayListTest() =
    inherit ListTest()

    override this.List array =
        upcast ArrayList array

    [<TestMethod; ExpectedException(typeof<ArgumentOutOfRangeException>)>]
    member this.InitialCapacityMustBeFinite() =
        ArrayList<Integer>(0) |> ignore
