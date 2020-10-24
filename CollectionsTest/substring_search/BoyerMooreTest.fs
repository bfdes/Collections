namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type BoyerMooreTest() =
    inherit SubstringSearchTest()

    override this.Algorithm pattern text = 
        SubstringSearch.BoyerMoore(pattern).Invoke text
