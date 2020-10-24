namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections
open System.Collections.Generic

[<TestClass>]
type MergeSortTest() =
    inherit SortingTest()

    override this.Sort array = 
        Sorting.MergeSort array
    
    [<TestMethod>]
    member this.ItSortsInAStableManner() =
        let pair() =
            (this.rnd.Next 5, this.rnd.Next 5)

        let arrays =
            let sample size =
                Array.init size (fun _ -> pair())

            let samples size = 
                let multiples = int(Math.Pow(3.0, float size))
                Seq.init multiples (fun _ -> sample size)

            Seq.init this.maxSize id
            |> Seq.collect samples

        let byI = Comparer<int*int>.Create(fun x y -> compare (fst x) (fst y))
        let byJ = Comparer<int*int>.Create(fun x y -> compare (snd x) (snd y))

        let test array = 
            Sorting.MergeSort(array, byJ)
            Sorting.MergeSort(array, byI)
            SortingTest.IsSorted array

        Assert.All test arrays
