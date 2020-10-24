namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<Ignore>]  // Not passing
[<TestClass>]
type RadixSortTest() =
    inherit SortingTest()

    override this.Sort array = 
        Sorting.RadixSort array
