namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type HeapSortTest() =
    inherit SortingTest()

    override this.Sort array = 
        Sorting.HeapSort array
