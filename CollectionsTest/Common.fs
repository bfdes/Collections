namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting

module Assert =
    let All (testCode: 't -> bool) (samples: seq<'t>): unit =
        let maybeFailed = Seq.tryFind (fun s ->  not (testCode s)) samples
        match maybeFailed with
        | Some(_) -> Assert.Fail()
        | _ -> ()

type Integer = Integer of int
         