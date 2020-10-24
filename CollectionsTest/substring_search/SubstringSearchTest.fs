namespace CollectionsTest

open System;
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections;

[<AbstractClass>]
type SubstringSearchTest() =
    let maxSize = 20
    let rnd = Random();
    
    abstract member Algorithm: string -> string -> int
    
    member this.Samples maxSize =
        let chars = Array.append [|'0'..'9'|] [|'a'..'z'|]
        let nextChar() = 
            chars.[rnd.Next(chars.Length-1)]

        let substrings (text: string) = 
            let n = text.Length
            seq {   for i in 0..n-1 do
                        for j in i..n-1 do
                            yield text.[i..j]
                }

        let text size =
            String [| for _ in 0..size-1 -> nextChar() |]

        let sample size = 
            let t = text size
            (t, substrings t)

        Seq.init maxSize sample

    [<TestMethod>]
    member this.ItFindsAllTheSubstrings() =
        let test (text, patterns) =
            let naive pattern =
                SubstringSearch.Naive(pattern).Invoke
            patterns
            |> Seq.map (fun p -> (naive p, this.Algorithm p))
            |> Seq.forall (fun (n, a) -> n text = a text)
        let samples = this.Samples maxSize
        Assert.All test samples
