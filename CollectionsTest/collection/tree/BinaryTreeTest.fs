namespace CollectionsTest

open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type BinaryTreeTest() =
    inherit TreeTest()

    override this.Tree pairs =
        let tree = BinarySearchTree()
        for (k, v) in pairs do
            tree.Put(k, v)
        upcast tree
