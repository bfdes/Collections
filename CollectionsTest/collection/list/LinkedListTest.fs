namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type LinkedListTest() =
    inherit ListTest()

    override this.List array =
        upcast LinkedList array
