namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type QuickSortTest() =
    inherit SortingTest()

    override this.Sort array = 
        Sorting.QuickSort array
