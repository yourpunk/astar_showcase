using AStarShowcase.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AStarShowcase.Unity
{
    public class PathfindingDemo :MonoBehaviour
    {
        public enum DragMode {None, DrawWalls, EraseWalls}
        [Header("References")]
        [SerializeField] private GridView gridView;
        [SerializeField] private Camera worldCamera;
 
        [Header("UI (optional — null-safe)")]
        [SerializeField] private Dropdown heuristicDropdown;
        [SerializeField] private Toggle allowDiagonalToggle;
        [SerializeField] private Toggle preventCornerCuttingToggle;
        [SerializeField] private Text statsLabel;
        private AStarPathfinder<GridNode> _pathfinder;
        private DragMode _dragMode = DragMode.None;
        private void Start()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            _pathfinder = new AStarPathfinder<GridNode>(gridView.Grid);
            SetupUI();
        }
        private void SetupUI()
        {
            if (heuristicDropdown != null)
            {
                heuristicDropdown.onValueChanged.AddListener(OnHeuristicChanged);
            }
            if (allowDiagonalToggle != null)
            {
                allowDiagonalToggle.isOn = gridView.Grid.AllowDiagonal;
                allowDiagonalToggle.onValueChanged.AddListener(value => 
                {
                    gridView.Grid.AllowDiagonal = value;
                    RecalculatePath();
                });
            }
                if (preventCornerCuttingToggle != null)
                {
                    preventCornerCuttingToggle.isOn = gridView.Grid.PreventCornerCutting;
                    preventCornerCuttingToggle.onValueChanged.AddListener(value =>
                    {
                        gridView.Grid.PreventCornerCutting = value;
                        RecalculatePath();
                    });
                }
            }
            private void OnHeuristicChanged(int index)
            {
                var type = (HeuristicType)index;
                gridView.Grid.HeuristicFunc = type switch
                {
                    HeuristicType.Manhattan => Heuristics.Manhattan,
                    HeuristicType.Euclidean => Heuristics.Euclidean,
                    HeuristicType.Diagonal => Heuristics.Diagonal,
                    HeuristicType.Chebyshev => Heuristics.Chebyshev,
                    _ => Heuristic.Diagonal
                };
                RecalculatePath();
            }
            private void Update()
            {
                HandleMouseInput();            
            }
            private void HandleMouseInput()
        {   
            Vector3 mouseWorld = worldCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            if (!gridView.WorldToCell(mouseWorld, out int x, out int y))
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    _dragMode = DragMode.None;
                    return;
                }
            }
            GridNode node = gridView.Grid.GetNode(x,y);
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    gridView.SetStart(node);
                    RecalculatePath();                    
                }
                else
                {
                    _dragMode =node.IsWalkable ? DragMode.DrawWalls : DragMode.EraseWalls;
                    ToggleWall(node);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                gridView.SetGoal(node);
                RecalculatePath();
            }
            if (Input.GetMouseButtonUp(0))
            {
                _dragMode = DragMode.None;
            }
        }
            private void ToggleWall(GridNode node)
        {
            if (node == gridView.StartNode || node == gridView.GoalNode) return;
            bool makeWall = _dragMode == DragMode.DrawWalls;
            gridView.Grid.SetWalkable(node.X, node.Y, !makeWall);
            RecalculatePath();
        }
        private void RecalculatePath()
        {
            if (gridView.StartNode == null || gridView.GoalNode == null) return;
            PathResult<GridNode> result = _pathfinder.FindPath(gridView.StartNode, gridView.GoalNode);
            gridView.DisplayResult(result);
            UpdateStatsLabel(result);
        }
        private void UpdateStatsLabel(PathResult<GridNode> result)
        {
            if (statslabel == null) return;
            statsLabel.text = result.Success  ? $"Path found: {result.Path.Count} cells | " +
                  $"Evaluated: {result.NodesEvaluated} nodes | " +
                  $"Time: {result.ElapsedMilliseconds:F3} ms"
                : $"No path found | Evaluated: {result.NodesEvaluated} nodes | " +
                  $"Time: {result.ElapsedMilliseconds:F3} ms";
        }
    }

}