using System;
using System.Collections.Generic;
namespace AStarShowcase.Core
{
    public class PathResult<TNode> where TNode : INode
    {
        public bool Success { get; }
        public List<TNode> Path { get; }
        public IReadOnlyCollection<TNode> ExploredNodes { get; }
        public int NodesEvaluated { get; }
        public double ElapsedMilliseconds { get; }
        public PathResult(bool success, List<TNode> path, IReadOnlyCollection<TNode> exploredNodes,
            int nodesEvaluated, double elapsedMilliseconds)
        {
            Success = success;
            Path = path;
            ExploredNodes = exploredNodes;
            NodesEvaluated = nodesEvaluated;
            ElapsedMilliseconds = elapsedMilliseconds;
        }
    }
    public class AStarPathfinder<TNode> where TNode : INode
    {
        private readonly IGrid<TNode> _grid;
        public AStarPathfinder(IGrid<TNode> grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        public PathResult<TNode> FindPath(TNode start, TNode goal)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            if (!start.IsWalkable || !goal.IsWalkable)
            {
                stopwatch.Stop();
                return new PathResult<TNode>(false, new List<TNode>(), new List<TNode>(), 0, stopwatch.Elapsed.TotalMilliseconds);
            }
            var gScore = new Dictionary<TNode, float> { [start] = 0f };
            var cameFrom = new Dictionary<TNode, TNode>();
            var fScore = new Dictionary<TNode, float> { [start] = _grid.GetHeuristic(start, goal) };
            var openSet = new BinaryHeap<TNode>((a, b) => fScore[a].CompareTo(fScore[b]));
            openSet.Insert(start);
            var closedSet = new HashSet<TNode>();
            int nodesEvaluated = 0;
            while (openSet.Count > 0)
            {
                TNode current = openSet.ExtractMin();
                nodesEvaluated++;
                if (Equals(current, goal))
                {
                    stopwatch.Stop();
                    var path = ReconstructPath(cameFrom, current);
                    return new PathResult<TNode>(true, path, closedSet, nodesEvaluated, stopwatch.Elapsed.TotalMilliseconds);
                }
                closedSet.Add(current);
                foreach (TNode neighbor in _grid.GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;
                    float tentativeGScore = gScore[current] + _grid.GetMovementCost(current, neighbor);
                    bool neighborIsNew = !gScore.ContainsKey(neighbor);
                    if (neighborIsNew || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + _grid.GetHeuristic(neighbor, goal);
                        if (neighborIsNew)
                            openSet.Insert(neighbor);
                        else
                            openSet.UpdatePriority(neighbor);
                    }
                }
            }
            stopwatch.Stop();
            return new PathResult<TNode>(false, new List<TNode>(), closedSet, nodesEvaluated, stopwatch.Elapsed.TotalMilliseconds);
        }
        private static List<TNode> ReconstructPath(Dictionary<TNode, TNode> cameFrom, TNode current)
        {
            var path = new List<TNode> { current };
            while (cameFrom.TryGetValue(current, out TNode previous))
            {
                current = previous;
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
