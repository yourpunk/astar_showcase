using System.Collections.Generic;

namespace AStarShowcase.Core
{
    public interface IGrid<TNode> where TNode : INode
    {
        IEnumerable<TNode> GetNeighbors(TNode node);
        float GetMovementCost(TNode from, TNode to);
        float GetHeuristic(TNode from, TNode to);
    }
}