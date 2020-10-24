namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type RabinKarpTest() =
    inherit SubstringSearchTest()

    member this.RabinKarp (pattern: string) = 
        let m = pattern.Length
        let monteCarlo = SubstringSearch.RabinKarp(pattern).Invoke

        let rec lasVegas (start: int)(text: string): int =
            let i = monteCarlo text.[start..]  // -1 for empty text
            if i = -1 then -1
            elif pattern.Equals text.[start+i..start+i+m-1] then start+i
            else lasVegas(start+i+1)(text)

        lasVegas 0

    override this.Algorithm pattern text =
        this.RabinKarp pattern text
