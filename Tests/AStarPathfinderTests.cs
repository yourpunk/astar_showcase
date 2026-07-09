using System.Linq;
using AStarShowcase.Core;
using NUnit.Framework;

namespace AStarShowcase.Tests
{
    public class AStarPathfinderTests
    {
        private static Grid2D CreateOpenGrid(int width, int height, bool allowDiagonal = true)
        {
            return new Grid2D(width, height, allowDiagonal);
        }
        
        [Test]
        public void FindPath_StraightLine_ReturnsShortestPath()
        {
            var grid = CreateOpenGrid(5, 1, allowDiagonal: false);
            var pathfinder = new AStarPathfinder<GridNode>(grid);
 
            var result = pathfinder.FindPath(grid.GetNode(0, 0), grid.GetNode(4, 0));
 
            Assert.IsTrue(result.Success);
            Assert.AreEqual(5, result.Path.Count);
        }
        [Test]
        public void FindPath_NoObstacles_PathLengthEqualsManhattanDistancePlusOne()
        {
            var grid = CreateOpenGrid(10,10,allowDiagonal: false);
            var pathfinder = new AStarPathfinder<GridNode>(grid);
            var start = grid.GetNode(0,0);
            var goal = grid.GetNode(5,5);
            var result = pathfinder.FindPath(start, goal);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(11, result.Path.Count);
        }
        [Test]
        public void FindPath_DiagonalAllowed_ShorterThanOrthogonalOnly()
        {
            var diagonalGrid = CreateOpenGrid(10,10,allowDiagonal: true);
            var orthogonalGrid = CreateOpenGrid(10,10, allowDiagonal: false);
            var diagonalResult = new AStarPathfinder<GridNode>(diagonalGrid).FindPath(diagonalGrid.GetNode(0,0), diagonalGrid.GetNode(5,5));
            var orthogonalResult = new AStarPathfinder<GridNode>(orthogonalGrid).FindPath(orthogonalGrid.GetNode(0,0), orthogonalGrid.GetNode(5,5));
            Assert.IsTrue(diagonalResult.Success);
            Assert.IsTrue(orthogonalResult.Success);
            Assert.Less(diagonalResult.Path.Count, orthogonalResult.Path.Count);
        }
        [Test]
        public void FindPath_CompleteWall_ReturnsFailure()
        {
            var grid = CreateOpenGrid(5,5,allowDiagonal: true);
            for (int y=0; y<5; y++) grid.SetWalkable(2,y,false);
            var pathfinder = new AStarPathfinder<GridNode>(grid);
            var result = pathfinder.FindPath(grid.GetNode(0,0), grid.GetNode(4,4));
            Assert.IsFalse(result.Success);
            Assert.AreEqual(0, result.Path.Count);
        }
        [Test]
        public void FindPath_UnwalkableStartOrGoal_ReturnsFailureImmediately()
        {
            var grid = CreateOpenGrid(5,5);
            grid.SetWalkable(0,0,false);
            var pathfinder = new AStarPathfinder<GridNode>(grid);
            var result = pathfinder.FindPath(grid.GetNode(0,0),grid.GetNode(4,4));
            Assert.IsFalse(result.Success);
            Assert.AreEqual(0, result.NodesEvaluated);
        }
        [Test]
        public void FindPath_SameStartAndGoal_ReturnsSingleNode()
        {
            var grid = CreateOpenGrid(5,5);
            var node = grid.GetNode(2,2);
            var pathfinder = new AStarPathfinder<GridNode>(grid);
            var result = pathfinder.FindPath(node, node);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Path.Count);
            Assert.AreEqual(node, result.Path[0]);
        }
        [Test]
        public void GetNeighbors_CornerCuttingPrevented_DiagonalThroughTwoWallsBlocked()
        {
            var grid = CreateOpenGrid(3,3,allowDiagonal: true);
            grid.PreventCornerCutting = true;
            grid.SetWalkable(1,0,false);
            grid.SetWalkable(0,1,false);
            var neighbors = grid.GetNeighbors(grid.GetNode(0, 0)).ToList();
            Assert.IsFalse(neighbors.Contains(grid.GetNode(1,1)),  "Diagonal move should be blocked when both orthogonal neighbors are walls.");
        }
        [Test]
        public void GetNeighbors_CornerCuttingAllowed_DiagonalThroughTwoWallsPermitted()
        {
            var grid = CreateOpenGrid(3,3,allowDiagonal: true);
            grid.PreventCornerCutting = false;
            grid.SetWalkable(1,0,false);
            grid.SetWalkable(0,1,false);
            var neighbors = grid.GetNeighbors(grid.GetNode(0, 0)).ToList();
            Assert.IsTrue(neighbors.Contains(grid.GetNode(1,1)), "Diagonal move should be permitted when corner-cutting prevention is disabled.");
        }
        [Test]
        public void GetNeighbors_CornerCuttingPrevented_DiagonalWithOneBlockedSideForbidden()
        {
            var grid = CreateOpenGrid(3,3,allowDiagonal: true);
            grid.PreventCornerCutting = true;
            grid.SetWalkable(1,0, false);
            var neighbors = grid.GetNeighbors(grid.GetNode(0, 0)).ToList();
            Assert.IsFalse(neighbors.Contains(grid.GetNode(1,1)),  "With strict corner-cutting prevention, one blocked orthogonal side is enough to forbid the diagonal.");
        }
        [Test]
        public void GetNeighbors_CornerCuttingPrevented_BothSidesOpenDiagonalAllowed()
        {
            var grid = CreateOpenGrid(3,3,allowDiagonal: true);
            grid.PreventCornerCutting = true;
            var neighbors = grid.GetNeighbors(grid.GetNode(0, 0)).ToList();
            Assert.IsTrue(neighbors.Contains(grid.GetNode(1, 1)),
                "Diagonal should be allowed when both orthogonal neighbors are walkable.");
        }
        [Test]
        public void FindPath_DifferentHeuristics_AllReturnSameOptimalCost()
        {
            var heuristics = new[] {HeuristicType.Manhattan, HeuristicType.Euclidean, HeuristicType.Diagonal};
            float? referenceCost = null;
            foreach (var h in heuristics)
            {
                var grid = CreateOpenGrid(8,8,allowDiagonal: false);
                grid.HeuristicFunc = h switch
                {
                    HeuristicType.Manhattan => Heuristics.Manhattan,
                    HeuristicType.Euclidean => Heuristics.Euclidean,
                    HeuristicType.Diagonal => Heuristics.Diagonal,
                    _ => Heuristics.Manhattan
                };
                var result = new AStarPathfinder<GridNode>(grid).FindPath(grid.GetNode(0,0), grid.GetNode(7,7));
                Assert.IsTrue(result.Success);
                float cost = result.Path.Count - 1;
                if (referenceCost == null) referenceCost = cost;
                else 
                    Assert.AreEqual(referenceCost.Value, cost, 0.001f,   $"Heuristic {h} produced a different optimal cost.");
            }
        }
    }
}