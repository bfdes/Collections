namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type LinkedQueueTest() =
    inherit QueueTest()

    override this.Queue array =
        upcast Queue array
