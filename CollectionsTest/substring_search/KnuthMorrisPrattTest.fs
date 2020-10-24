namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections
open System

[<TestClass>]
type KnuthMorrisPrattTest() =
    inherit SubstringSearchTest()

    override this.Algorithm pattern text = 
        SubstringSearch.KnuthMorrisPratt(pattern).Invoke text
