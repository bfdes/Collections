namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type RedBlackTreeTest() =
    inherit TreeTest()

    override this.Tree pairs =
        let tree = RedBlackTree()
        for (k, v) in pairs do
            tree.Put(k, v)
        upcast tree
