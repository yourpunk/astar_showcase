using System.Collections.Generic;
using AStarShowcase.Core;
using UnityEngine;
namespace AStarShowcase.Unity
{
    public class GridView : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 30;
        [SerializeField] private int height = 20;
        [SerializeField] private float cellSize = 1f;
        [Header("Colors")]
        [SerializeField] private Color walkableColor  = new Color(0.85f, 0.85f, 0.85f);
        [SerializeField] private Color wallColor      = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color startColor     = Color.green;
        [SerializeField] private Color goalColor      = Color.red;
        [SerializeField] private Color pathColor      = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color openSetColor   = new Color(0.4f, 0.7f, 1f, 0.5f);
        [SerializeField] private Color closedSetColor = new Color(0.3f, 0.3f, 0.9f, 0.35f);
        public Grid2D   Grid      { get; private set; }
        public GridNode StartNode { get; private set; }
        public GridNode GoalNode  { get; private set; }
        private List<GridNode>              _currentPath;
        private IReadOnlyCollection<GridNode> _exploredNodes;
        public Vector3 CellToWorld(int x, int y)
        {
            return transform.position + new Vector3((x + 0.5f) * cellSize, (y + 0.5f) * cellSize, 0f);
        }
        public bool WorldToCell(Vector3 worldPos, out int x, out int y)
        {
            Vector3 localPos = worldPos - transform.position;
            x = Mathf.FloorToInt(localPos.x / cellSize);
            y = Mathf.FloorToInt(localPos.y / cellSize);
            return Grid != null && Grid.InBounds(x, y);
        }
        private void Awake()
        {
            Grid = new Grid2D(width, height, allowDiagonal: true);
        }
        public void SetStart(GridNode node) => StartNode = node;
        public void SetGoal(GridNode node) => GoalNode = node;
        public void DisplayResult(PathResult<GridNode> result)
        {
            _currentPath   = result.Path;
            _exploredNodes = result.ExploredNodes;
        }
        public void ClearVisualization()
        {
            _currentPath   = null;
            _exploredNodes = null;
        }
        private void OnDrawGizmos()
        {
            if (Grid == null) return;
            float halfCell = cellSize * 0.5f;
            var pathSet     = _currentPath   != null ? new HashSet<GridNode>(_currentPath)   : null;
            var exploredSet = _exploredNodes != null ? new HashSet<GridNode>(_exploredNodes) : null;
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    GridNode node   = Grid.GetNode(x, y);
                    Vector3  center = CellToWorld(x, y);
                    Color color;
                    if (!node.IsWalkable)
                        color = wallColor;
                    else if (pathSet != null && pathSet.Contains(node))
                        color = pathColor;
                    else if (exploredSet != null && exploredSet.Contains(node))
                        color = closedSetColor;
                    else
                        color = walkableColor;
                    Gizmos.color = color;
                    Gizmos.DrawCube(center, new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.01f));
                }
            }
            Gizmos.color = new Color(0f, 0f, 0f, 0.15f);
            for (int x = 0; x <= Grid.Width; x++)
            {
                Vector3 from = transform.position + new Vector3(x * cellSize, 0, 0);
                Vector3 to   = from + new Vector3(0, Grid.Height * cellSize, 0);
                Gizmos.DrawLine(from, to);
            }
            for (int y = 0; y <= Grid.Height; y++)
            {
                Vector3 from = transform.position + new Vector3(0, y * cellSize, 0);
                Vector3 to   = from + new Vector3(Grid.Width * cellSize, 0, 0);
                Gizmos.DrawLine(from, to);
            }
            if (StartNode != null)
            {
                Gizmos.color = startColor;
                Gizmos.DrawSphere(CellToWorld(StartNode.X, StartNode.Y), halfCell * 0.6f);
            }
            if (GoalNode != null)
            {
                Gizmos.color = goalColor;
                Gizmos.DrawSphere(CellToWorld(GoalNode.X, GoalNode.Y), halfCell * 0.6f);
            }
            if (_currentPath != null && _currentPath.Count > 1)
            {
                Gizmos.color = pathColor;
                for (int i = 0; i < _currentPath.Count - 1; i++)
                {
                    Vector3 a = CellToWorld(_currentPath[i].X,     _currentPath[i].Y);
                    Vector3 b = CellToWorld(_currentPath[i + 1].X, _currentPath[i + 1].Y);
                    Gizmos.DrawLine(a, b);
                }
            }
        }
    }
}