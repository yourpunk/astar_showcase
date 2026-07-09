using System;
using System.Collections.Generic;

namespace AStarShowcase.Core
{
    public class Grid2D : IGrid<GridNode>
    {
        private readonly GridNode[,] _nodes;
        public int Width { get;}
        public int Height { get;}
        public bool AllowDiagonal { get; set;}
        public bool PreventCornerCutting { get; set;} = true;
        public Func<int, int, float> HeuristicFunc {get; set;} = Heuristics.Diagonal;
        private const float StraightCost = 1f;
        private const float DiagonalCost = 1.41421356f; // √2
        public Grid2D(int width, int height, bool allowDiagonal = true)
        {
            if (width <= 0 || height <=0) throw new ArgumentException("Grid dimensions must be positive.");
            Width = width;
            Height = height;
            AllowDiagonal = allowDiagonal;

            _nodes = new GridNode[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _nodes[x, y] = new GridNode(x, y);
        }
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public GridNode GetNode( int x, int y)
        {
            if (!InBounds(x, y)) throw new ArgumentOutOfRangeException($"({x}, {y}) is outside the grid.");
            return _nodes[x, y];
        }

        public void SetWalkable(int x, int y, bool walkable)
        {
            if (InBounds(x, y)) _nodes[x, y].IsWalkable = walkable;
        }
        private static readonly (int dx, int dy)[] StraightDirs =
        {
            (1,0), (-1,0), (0,1), (0,-1)
        };
        private static readonly (int dx, int dy)[] DiagonalDirs =
        {
            (1, 1), (1, -1), (-1, 1), (-1,-1)
        };
        public IEnumerable<GridNode> GetNeighbors(GridNode node)
        {
            foreach (var (dx, dy) in StraightDirs)
            {
                int nx = node.X + dx;
                int ny = node.Y + dy;
                if (InBounds(nx, ny) && _nodes[nx, ny].IsWalkable) 
                    yield return _nodes[nx, ny];
            }
            if (!AllowDiagonal) yield break;
            foreach (var (dx, dy) in DiagonalDirs)
            {
                int nx = node.X + dx;
                int ny = node.Y + dy;
                if (!InBounds(nx, ny) || !_nodes[nx, ny].IsWalkable) continue;
                if (PreventCornerCutting)
                {
                    bool horizontalOpen = InBounds(node.X + dx, node.Y) && _nodes[node.X + dx, node.Y].IsWalkable;

                    bool verticalOpen = InBounds(node.X, node.Y + dy) && _nodes[node.X, node.Y + dy].IsWalkable;
                    if (!horizontalOpen || !verticalOpen) continue;
                }
                yield return _nodes[nx, ny];
            }
        }
        public float GetMovementCost(GridNode from, GridNode to)
        {
            bool isDiagonal = from.X != to.X && from.Y != to.Y;
            float baseCost = isDiagonal ? DiagonalCost : StraightCost; 
            float terrainMultiplier = (from.MovementCost + to.MovementCost) * 0.5f;
            return baseCost * terrainMultiplier;
        }

        public float GetHeuristic(GridNode from, GridNode to)
        {
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;
            return HeuristicFunc(dx, dy);
        }
        
    }
}