namespace CollectionsTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open global.Collections

[<TestClass>]
type KdTreeTest() =
    let maxSize = 10
    let maxValue = float 100
    let rnd = Random()

    let Points size = 
        let nextDouble() =
            2.0 * maxValue * rnd.NextDouble() - maxValue

        let nextPoint() =
            Point(nextDouble(), nextDouble(), nextDouble())

        Array.init size (fun _ -> nextPoint())

    member this.Samples = 
        Seq.init maxSize (fun i -> i+1)
        |> Seq.map Points
    
    [<TestMethod>]
    member this.ItMaintainsSymmetricOrder() =
        let test (points: array<Point>) =
            let tree = KdTree points
            tree.IsSymmetric()

        Assert.All test this.Samples

    [<TestMethod>]
    member this.ItCountainsAllThePoints() = 
        let test (points: array<Point>) =
            let tree = KdTree points
            Seq.forall tree.Contains points

        Assert.All test this.Samples

    [<TestMethod>]
    member this.DeleteTest() =
        let test (points: array<Point>) =
            let tree = KdTree points
            Seq.iter tree.Delete points
            tree.IsEmpty()

        Assert.All test this.Samples

    [<TestMethod>]
    member this.PointsAreTheirOwnNearestNeighbours() = 
        let test (points: array<Point>) = 
            let tree = KdTree points
            Seq.forall (fun p -> p = tree.NearestNeighbour p) points
        
        Assert.All test this.Samples

    [<TestMethod>]
    member this.NearestNeighbourSearchTest() =
        // Test whether the implementation of NN search produces the same result as a linear scan
        let closest p =
            Array.reduce<Point>(fun min q -> if q.DistanceSquaredTo(p) < p.DistanceSquaredTo(min) then q else min)

        let test (points: array<Point>) = 
            let tree = KdTree points
            let args = Points points.Length  // The choice to generate this many args is arbitrary
            Seq.forall (fun p -> closest p points = tree.NearestNeighbour p) args

        Assert.All test this.Samples

    [<TestMethod>]
    member this.BoundingBoxContainsAllThePoints() =
        let boundingBox = Rectangle.InfiniteBox()

        let test (points: array<Point>) = 
            let tree = KdTree points
            points.Length = Seq.length(tree.RangeSearch boundingBox)

        Assert.All test this.Samples
