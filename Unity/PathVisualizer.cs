using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStarShowcase.Core;

namespace AStarShowcase.Unity
{
    public class PathVisualizer : Monobehaviour
    {
        [SerializeField] private GridView gridView;
        [SerializeField] private Transform agentMarker;
        [SerializeField] private float moveSpeed = 4f;
        private Coroutine _activeRoutine;
        public void AnimatePath(List<GridNode> path)
        {
            if (_activeRoutine != null) StopCoroutine(_activeRoutine);
            if (agentMarker == null || path == null || path.Count == 0) return;
            _activeRoutine = StartCoroutine(MoveAlongPath(path));
        }
        private IEnumerator MoveAlongPath(List<GridNode> path)
        {
            agentMarker.position = gridView.CellToWorld(path[0].X, path[0].Y);
            for (int i = 1; i < path.Count; i++)
            {
                Vector3 from = agentMarker.position;
                Vector3 to= gridView.CellToWorld(path[i].X, path[i].Y);
                float distance = Vector3.Distance(from, to);
                float duration = distance / moveSpeed;
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    agentMarker.position = Vector3.Lerp(from, to, elapsed/duration);
                    yield return null;
                }
                agentMarker.position = to;
            }
            _activeRoutine = null;
        }
    }
}