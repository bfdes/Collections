namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type InsertionSortTest() =
    inherit SortingTest()

    override this.Sort array = 
        Sorting.InsertionSort array        
    